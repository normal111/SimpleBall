using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBallPoolingManager : MonoBehaviour
{
    public GameObject[] m_NewBalls;

    private int m_NewBallIndex;

    void Awake()
    {
        m_NewBallIndex = 0;
    }

    public GameObject SetNewBall(Vector2 vector2)
    {
        GameObject newBall = m_NewBalls[m_NewBallIndex];
        m_NewBallIndex = (m_NewBallIndex + 1) % m_NewBalls.Length;
        newBall.transform.position = vector2;
        newBall.SetActive(true);
        return newBall;
    }
}
