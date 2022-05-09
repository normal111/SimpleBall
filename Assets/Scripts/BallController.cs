using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private int m_RetrieveCount = 0;

    public Vector2 m_ShootingPosition;

    public GameObject m_BallPrefab;
    public List<Rigidbody2D> m_BallList;
    public float m_MaxX;
    public TextMeshProUGUI m_BallCountText;
    public Vector3 m_TextPosOffset = new Vector3(0, -1);
    public float m_BallSpeed = 800f;
    public float m_MinYVelocity = 0.2f;

    void Start()
    {
        SetBallCountText(m_BallList.Count, m_ShootingPosition);
    }

    // 모든 공 발사
    public IEnumerator LaunchAllBall(Vector3 direction, Action callback)
    {
        m_RetrieveCount = 0;

        Vector3 initPos = m_ShootingPosition;

        // 공 발사
        for (int i = 0; i < m_BallList.Count; i++)
        {
            SetBallCountText(m_BallList.Count - i - 1, initPos);
            m_BallList[i].isKinematic = false;
            m_BallList[i].AddForce(direction * m_BallSpeed);
            StartCoroutine(CheckHorizontal(m_BallList[i]));
            yield return StartCoroutine(WaitFixedUpdate(3));
        }

        // 공 수신 대기
        while (m_RetrieveCount < m_BallList.Count)
        {
            yield return null;
        }

        SetBallCountText(m_BallList.Count, m_ShootingPosition);
        callback?.Invoke();
    }

    // 공 수평 이동 검사
    private IEnumerator CheckHorizontal(Rigidbody2D rigidbody2D)
    {
        yield return new WaitForSeconds(1f);
        while (rigidbody2D.isKinematic == false)
        {
            if (-m_MinYVelocity < rigidbody2D.velocity.y && rigidbody2D.velocity.y < m_MinYVelocity)
            {
                rigidbody2D.AddForce(Vector2.down * 1000f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // 발사한 공 바닥에 도착
    public Vector2 RetrieveBall(float x)
    {
        if (m_RetrieveCount == 0)   // 처음으로 떨어진 공의 위치 지정
        {
            if (m_MaxX < x)
            {
                m_ShootingPosition.x = m_MaxX;
            }
            else if (x < -m_MaxX)
            {
                m_ShootingPosition.x = -m_MaxX;
            }
            else
            {
                m_ShootingPosition.x = x;
            }
        }
        m_RetrieveCount++;

        return m_ShootingPosition;
    }

    // FixedUpdate 한프레임 기다림
    private IEnumerator WaitFixedUpdate(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return waitForFixedUpdate;
        }
    }

    // 공 추가
    public void AddBall(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject ball = Instantiate(m_BallPrefab, m_ShootingPosition, Quaternion.identity, this.transform);
            ball.GetComponent<Ball>().m_BallController = this;
            m_BallList.Add(ball.GetComponent<Rigidbody2D>());
        }
        SetBallCountText(m_BallList.Count, m_ShootingPosition);
    }

    private void SetBallCountText(int count, Vector3 launchPosition)
    {
        if (count <= 0)
        {
            m_BallCountText.text = "";
        }
        else
        {
            m_BallCountText.text = "x" + count;
            m_BallCountText.transform.position = launchPosition + m_TextPosOffset;
        }
    }
}
