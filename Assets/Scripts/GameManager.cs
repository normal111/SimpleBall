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
        // ���� ��� �ҷ�����
        m_MaxScore = 5;
        // ���� �� ����, ����, �߻� ����, �� ����, �߻� ���� �� �߻� ����
        m_MapBlockList = new List<GameObject>();
        m_MapNewBallList = new List<GameObject>();
        m_BallController.AddBall(1);

        m_IsLaunching = true;
        SetScore(0);
        StartCoroutine(FinishRound());
    }

    // ��ġ ����
    void Update()
    {
        if (m_IsLaunching)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            startingTouchPoint = Input.mousePosition;
            startingTouchPoint.z = 10f;     // ī�޶� z offset
            m_IsTouching = true;
        }

        if (m_IsTouching)
        {
            currentTouchPoint = Input.mousePosition;
            currentTouchPoint.z = 10f;      // ī�޶� z offset

            if (Vector2.Distance(currentTouchPoint, startingTouchPoint) < 10f)  // �̼��� �������� ����
                return;

            m_TouchLineRenderer.gameObject.SetActive(true);     // ��ġ �� ��Ÿ���� �������� �ѱ�

            TouchPointOffset = currentTouchPoint - startingTouchPoint;
            float angle = Mathf.Atan2(TouchPointOffset.y, TouchPointOffset.x) * Mathf.Rad2Deg;

            // ��ġ ����
            m_TouchLineRenderer.SetPosition(0, Camera.main.ScreenToWorldPoint(startingTouchPoint));
            m_TouchLineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(currentTouchPoint));

            if (170f < angle || angle < -90f)   // ���� ����
            {
                angle = 170f;
            }
            else if (angle < 10f)               // ���� ����
            {
                angle = 10f;
            }

            float TouchPointOffsetX = Mathf.Tan(-(angle + 90f) * Mathf.Deg2Rad);
            TouchPointOffset.x = TouchPointOffsetX;
            TouchPointOffset.y = 1f;

            // �� ���� ȭ��ǥ
            arrowAngle.z = angle;
            m_Arrow.eulerAngles = arrowAngle;
            m_Arrow.position = m_BallController.m_LauchPosition;

            // �� ����
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

    // ���� ���� �۾�
    private IEnumerator FinishRound()
    {
        yield return null;
        // ���� �� ����
        SetScore(m_Score + 1);
        List<GameObject> blockList = m_BlockPoolingManager.GetBlocks(GetBlockCount(m_Score));
        SetBlocks(blockList, m_Score);
        m_MapBlockList.AddRange(blockList);

        // �ʷ� �� ����
        m_MapNewBallList.Add(m_NewBallPoolingManager.SetNewBall(GetRandomPoint()));

        // ��� �� ������
        yield return StartCoroutine(LowerMap(m_MapBlockList, m_MapNewBallList));

        // ���� ���� �˻�
        if (CheckGameOver(m_MapBlockList, m_FloorLevelHeight))
        {
            m_CameraAnimator.SetTrigger("Shake");
            m_GameOverPanel.SetActive(true);
            m_ResultScoreText.text = "���� ����: " + m_Score;
            yield break;
        }

        // �ٴ� ��ó���� ������ NewBall ȸ��
        yield return StartCoroutine(CheckNewBall(m_MapNewBallList, m_FloorLevelHeight));

        // �ٴڿ� ������ NewBall ȸ��
        yield return StartCoroutine(RetrieveNewBall());

        m_IsLaunching = false;
    }

    // ���� ���� �� �ؽ�Ʈ ����
    private void SetScore(int newScore)
    {
        m_Score = newScore;
        m_ScoreText.text = "���� ����: " + m_Score;

        if (m_MaxScore < m_Score)
        {
            m_MaxScore = m_Score;
        }
        m_MaxScoreText.text = "�ְ� ����: " + m_MaxScore;
    }

    #region �� ��ġ
    // ���� ���� ��ġ�� ��ġ
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

    // ������ ���� ���� ���� ����
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

    // ������ �� ���� ��ġ ���
    private Vector2 GetRandomPoint()
    {
        int index = UnityEngine.Random.Range(0, m_BlockPoints.Length);
        return m_BlockPoints[index].position;
    }
    #endregion

    #region �� ������
    // �� ��ü�� ������
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

    // ������Ʈ ������
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
    // ȹ���� NewBall ����Ʈ�� �߰�
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

    // NewBall �ٴ� ��ó���� �˻�
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

    // �̹� ���� ȹ���� NewBall ȸ��
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

    // ������ NewBall�� �߻� ��ġ�� �̵�
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
