using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    private int m_HP;
    private int m_MaxHP = 0;

    public Text m_HPText;
    public Animator m_Animator;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            m_Animator.SetTrigger("Hit");
            SetHP(m_HP - 1);
            if (m_HP <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    internal void SetHP(int score/*, int maxScore = m_MaxHP*/)
    {
        m_HP = score;
        m_HPText.text = m_HP.ToString();
    }
}
