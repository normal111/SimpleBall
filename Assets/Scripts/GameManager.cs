using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int m_Score;
    private int m_MaxScore;
    private bool m_IsTouching = false;
    private bool m_IsLaunching;
    private List<NewBall> m_FalledNewBallList;

    private Vector3 startingTouchPoint;
    private Vector3 currentTouchPoint;
    private Vector3 TouchPointOffset;
    private Vector3 arrowAngle = Vector3.zero;
    private List<GameObject> m_MapBlockList;
    private List<GameObject> m_MapNewBallList;

    public BallController m_BallController;
    public BlockPoolingManager m_BlockPoolingManager;
    public NewBallPoolingManager m_NewBallPoolingManager;
    public LineRenderer m_TouchLineRenderer;
    public LineRenderer m_TrajectoryLineRenderer;
    public Transform m_Arrow;
    public Transform m_ExpectBall;
    public Text m_ScoreText;
    public Text m_MaxScoreText;
    public Transform[] m_BlockPoints;
    public float m_FloorLevelHeight = -5f;
    public float m_OneLevelHeight = 1.2f;
    public GameObject m_GameOverPanel;
    public Animator m_CameraAnimator;
    public Text m_ResultScoreText;

    private void Awake()
    {
        m_GameOverPanel.SetActive(false);

        m_FalledNewBallList = new List<NewBall>();
        m_TouchLineRenderer.gameObject.SetActive(false);
        // 이전 기록 불러오기
        m_MaxScore = 5;
        // 이전 맵 정보, 점수, 발사 지점, 공 개수, 발사 여부 및 발사 각도
        m_MapBlockList = new List<GameObject>();
        m_MapNewBallList = new List<GameObject>();
        m_BallController.AddBall(1);

        m_IsLaunching = true;
        SetScore(0);
        StartCoroutine(FinishRound());
    }

    // 터치 감지
    void Update()
    {
        if (m_IsLaunching)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            startingTouchPoint = Input.mousePosition;
            startingTouchPoint.z = 10f;     // 카메라 z offset
            m_IsTouching = true;
        }

        if (m_IsTouching)
        {
            currentTouchPoint = Input.mousePosition;
            currentTouchPoint.z = 10f;      // 카메라 z offset

            if (Vector2.Distance(currentTouchPoint, startingTouchPoint) < 10f)  // 미세한 움직임은 무시
                return;

            m_TouchLineRenderer.gameObject.SetActive(true);     // 터치 중 나타나는 오브젝터 켜기

            TouchPointOffset = currentTouchPoint - startingTouchPoint;
            float angle = Mathf.Atan2(TouchPointOffset.y, TouchPointOffset.x) * Mathf.Rad2Deg;

            // 터치 라인
            m_TouchLineRenderer.SetPosition(0, Camera.main.ScreenToWorldPoint(startingTouchPoint));
            m_TouchLineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(currentTouchPoint));

            if (170f < angle || angle < -90f)   // 상한 각도
            {
                angle = 170f;
            }
            else if (angle < 10f)               // 하한 각도
            {
                angle = 10f;
            }

            float TouchPointOffsetX = Mathf.Tan(-(angle + 90f) * Mathf.Deg2Rad);
            TouchPointOffset.x = TouchPointOffsetX;
            TouchPointOffset.y = 1f;

            // 공 방향 화살표
            arrowAngle.z = angle;
            m_Arrow.eulerAngles = arrowAngle;
            m_Arrow.position = m_BallController.m_LauchPosition;

            // 공 궤적
            m_ExpectBall.position = Physics2D.CircleCast(m_BallController.m_LauchPosition, 0.25f, TouchPointOffset.normalized, 100, 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Block")).centroid;
            m_TrajectoryLineRenderer.SetPosition(0, m_BallController.m_LauchPosition);
            m_TrajectoryLineRenderer.SetPosition(1, m_ExpectBall.position);
        }

        if (m_IsTouching && Input.GetKeyUp(KeyCode.Mouse0))
        {
            m_TouchLineRenderer.gameObject.SetActive(false);
            m_IsTouching = false;
            m_IsLaunching = true;
            StartCoroutine(m_BallController.LaunchAllBall(TouchPointOffset.normalized, () => StartCoroutine(FinishRound())));
        }
    }

    // 라운드 종료 작업
    private IEnumerator FinishRound()
    {
        yield return null;
        // 다음 블럭 생성
        SetScore(m_Score + 1);
        List<GameObject> blockList = m_BlockPoolingManager.GetBlocks(GetBlockCount(m_Score));
        SetBlocks(blockList, m_Score);
        m_MapBlockList.AddRange(blockList);

        // 초록 공 생성
        m_MapNewBallList.Add(m_NewBallPoolingManager.SetNewBall(GetRandomPoint()));

        // 모든 블럭 내리기
        yield return StartCoroutine(LowerMap(m_MapBlockList, m_MapNewBallList));

        // 게임 종료 검사
        if (CheckGameOver(m_MapBlockList, m_FloorLevelHeight))
        {
            m_CameraAnimator.SetTrigger("Shake");
            m_GameOverPanel.SetActive(true);
            m_ResultScoreText.text = "최종 점수: " + m_Score;
            yield break;
        }

        // 바닥 근처까지 내려온 NewBall 회수
        yield return StartCoroutine(CheckNewBall(m_MapNewBallList, m_FloorLevelHeight));

        // 바닥에 떨어진 NewBall 회수
        yield return StartCoroutine(RetrieveNewBall());

        m_IsLaunching = false;
    }

    // 점수 수정 및 텍스트 수정
    private void SetScore(int newScore)
    {
        m_Score = newScore;
        m_ScoreText.text = "현재 점수: " + m_Score;

        if (m_MaxScore < m_Score)
        {
            m_MaxScore = m_Score;
        }
        m_MaxScoreText.text = "최고 점수: " + m_MaxScore;
    }

    #region 블럭 배치
    // 블럭을 생성 위치에 배치
    private void SetBlocks(List<GameObject> blockList, int score)
    {
        List<int> indexList = Enumerable.Range(0, m_BlockPoints.Length).ToList();
        for (int i = 0; i < blockList.Count; i++)
        {
            int index = UnityEngine.Random.Range(0, indexList.Count);
            blockList[i].transform.position = m_BlockPoints[indexList[index]].position;
            indexList.RemoveAt(index);
            blockList[i].GetComponent<Block>().SetHP(score);
            blockList[i].SetActive(true);
        }
    }

    // 점수에 따른 생성 블럭수 지정
    private int GetBlockCount(int score)
    {
        int count;
        if (score % 20 == 0)
        {
            return 6;
        }
        int randBlock = UnityEngine.Random.Range(0, 24);
        if (score <= 10)
            count = randBlock < 16 ? 1 : 2;
        else if (score <= 20)
            count = randBlock < 8 ? 1 : (randBlock < 16 ? 2 : 3);
        else if (score <= 40)
            count = randBlock < 9 ? 2 : (randBlock < 18 ? 3 : 4);
        else
            count = randBlock < 8 ? 2 : (randBlock < 16 ? 3 : (randBlock < 20 ? 4 : 5));
        return count;
    }

    // 무작위 블럭 생성 위치 얻기
    private Vector2 GetRandomPoint()
    {
        int index = UnityEngine.Random.Range(0, m_BlockPoints.Length);
        return m_BlockPoints[index].position;
    }
    #endregion

    #region 맵 내리기
    // 맵 전체를 내리기
    private IEnumerator LowerMap(List<GameObject> mapBlockList, List<GameObject> mapNewBallList)
    {
        float lowerTime = 0.25f;

        for (int i = 0; i < mapBlockList.Count; i++)
        {
            if (mapBlockList[i].activeSelf == false)
            {
                mapBlockList.RemoveAt(i);
                i--;
                continue;
            }
            StartCoroutine(LowerObject(mapBlockList[i].transform, m_OneLevelHeight, lowerTime));
        }

        for (int i = 0; i < mapNewBallList.Count; i++)
        {
            if (mapNewBallList[i].activeSelf == false)
            {
                mapNewBallList.RemoveAt(i);
                i--;
                continue;
            }

            StartCoroutine(LowerObject(mapNewBallList[i].transform, m_OneLevelHeight, lowerTime));
        }

        yield return new WaitForSeconds(lowerTime);
    }

    // 오브젝트 내리기
    private IEnumerator LowerObject(Transform transform, float distance, float duration)
    {
        Vector2 origin = transform.position;

        Vector2 target = origin;
        target.y -= distance;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            transform.position = Vector2.Lerp(origin, target, time / duration);
            yield return null;
        }
        transform.position = target;
    }

    private bool CheckGameOver(List<GameObject> mapBlockList, float minHeight)
    {
        for (int i = 0; i < mapBlockList.Count; i++)
        {
            if (mapBlockList[i].transform.position.y < minHeight)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region NewBall
    // 획득한 NewBall 리스트에 추가
    public void AddFalledNewBall(NewBall newBall)
    {
        if (m_MapNewBallList.Contains(newBall.gameObject))
        {
            m_MapNewBallList.Remove(newBall.gameObject);
        }

        if (m_FalledNewBallList.Contains(newBall) == false)
        {
            m_FalledNewBallList.Add(newBall);
        }
    }

    // NewBall 바닥 근처인지 검사
    private IEnumerator CheckNewBall(List<GameObject> mapNewBallList, float height)
    {
        for (int i = 0; i < mapNewBallList.Count; i++)
        {
            if (mapNewBallList[i].transform.position.y < height)
            {
                mapNewBallList[i].GetComponent<NewBall>().HitBall();
            }
        }

        yield return null;
    }

    // 이번 라운드 획득한 NewBall 회수
    private IEnumerator RetrieveNewBall()
    {
        if (m_FalledNewBallList.Count == 0)
        {
            yield break;
        }

        for (int i = 0; i < m_FalledNewBallList.Count; i++)
        {
            StartCoroutine(GatherNewball(m_FalledNewBallList[i].transform, m_BallController.m_LauchPosition));
        }
        yield return new WaitForSeconds(0.25f);
        m_BallController.AddBall(m_FalledNewBallList.Count);
        m_FalledNewBallList.Clear();
    }

    // 떨어진 NewBall을 발사 위치로 이동
    private IEnumerator GatherNewball(Transform transform, Vector2 target)
    {
        Vector2 origin = transform.position;
        float duration = Vector2.Distance(origin, target) / 20;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            transform.position = Vector2.Lerp(origin, target, time / duration);
            yield return null;
        }
        transform.position = target;
        transform.gameObject.SetActive(false);
    }
    #endregion

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
