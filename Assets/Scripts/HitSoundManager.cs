using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoundManager : MonoBehaviour
{
    public static HitSoundManager instance;

    public AudioSource[] m_AudioChannels;

    private int m_AudioChannelIndex;

    private void Awake()
    {
        instance = this;
        m_AudioChannelIndex = 0;
    }

    public void PlayHitSound()
    {
        if (m_AudioChannels[m_AudioChannelIndex].isPlaying == false || 0.08f < m_AudioChannels[m_AudioChannelIndex].time)
        {
            m_AudioChannels[m_AudioChannelIndex].Play();
            m_AudioChannelIndex = (m_AudioChannelIndex + 1) % m_AudioChannels.Length;
        }
    }
}
