using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public enum BlockType { Square, LeftTri, RightTri, Ellipse }

    public int m_HP;
    public int m_MaxHP = 1;

    public TextMeshProUGUI m_HPText;
    public Animator m_Animator;
    public SpriteRenderer m_Sprite;
    public ParticleSystem m_BreakParticle;
    public Color[] m_Colors;
    public BlockType m_BlockType;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            HitSoundManager.instance.PlayHitSound();
            m_Animator.SetTrigger("Hit");
            SetHP(m_HP - 1);
            if (m_HP <= 0)
            {
                m_BreakParticle.transform.parent = null;
                m_BreakParticle.Play();
                this.gameObject.SetActive(false);
            }
        }
    }

    public void SetMaxHP(int maxHP)
    {
        m_MaxHP = maxHP;
    }

    public void SetHP(int hp)
    {
        m_HP = hp;
        m_HPText.text = m_HP.ToString();
        m_Sprite.color = GetColor(m_HP, m_MaxHP);
    }

    private Color GetColor(float value, float max)
    {
        return Color.Lerp(m_Colors[0], m_Colors[1], value / max);
    }
}
