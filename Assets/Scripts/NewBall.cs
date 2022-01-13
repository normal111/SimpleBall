using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBall : MonoBehaviour
{
    public Collider2D m_Collider2D;
    public GameManager m_GameManager;
    public Animator m_Animator;
    public ParticleSystem m_GetParticle;

    private void OnEnable()
    {
        m_Collider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            HitBall();
        }
    }

    public void HitBall()
    {
        m_GetParticle.Play();
        m_Collider2D.enabled = false;
        m_Animator.SetTrigger("Hit");
        m_GameManager.AddFalledNewBall(this);
        StartCoroutine(MoveTo(new Vector2(this.transform.position.x, m_GameManager.m_BallController.m_LauchPosition.y)));
    }

    // ¹Ù´Ú¿¡ ¶³¾îÁö±â
    private IEnumerator MoveTo(Vector2 target)
    {
        Vector2 origin = this.transform.position;
        float duration = Vector2.Distance(origin, target) / 10f;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            this.transform.position = Vector2.Lerp(origin, target, time / duration);
            yield return null;
        }
        this.transform.position = target;
    }
}
