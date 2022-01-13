using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public BallController m_BallController;
    public Rigidbody2D m_Rigidbody2D;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            m_Rigidbody2D.isKinematic = true;
            m_Rigidbody2D.velocity = Vector2.zero;

            StartCoroutine(Move(m_BallController.RetrieveBall(this.transform.position.x)));
        }
    }

    private IEnumerator Move(Vector3 target)
    {
        Vector2 origin = this.transform.position;
        float duration = Vector2.Distance(origin, target) / 20f;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            this.transform.position = Vector2.Lerp(origin, target, time / duration);
            yield return null;
        }

        this.transform.position = target;
    }

    //private void Update()
    //{
    //    if (m_Rigidbody2D.isKinematic == false)
    //    {
    //        if (-0.05f < m_Rigidbody2D.velocity.y && m_Rigidbody2D.velocity.y < 0.05f)
    //        {
    //            m_Rigidbody2D.AddForce(Vector2.down * 1000f);
    //        }
    //    }
    //}
}
