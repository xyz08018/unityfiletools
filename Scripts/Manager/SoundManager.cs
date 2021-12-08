using System;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    private AudioSource m_BGMSource;
    private AudioSource m_SoundSource;

    void Start()
    {
        m_BGMSource = GameObject.Find("SoundManager").transform.GetComponents<AudioSource>()[0];
        m_SoundSource = GameObject.Find("SoundManager").transform.GetComponents<AudioSource>()[1];
    }

    public void PlayBGM()
    {
        if (m_BGMSource.isPlaying)
            m_BGMSource.Stop();
        m_BGMSource.Play();
        m_BGMSource.loop = true;
    }
    public void StopBMG()
    {
        m_BGMSource.Stop();
    }


    public float PlaySound(AudioClip clip)
    {
        if (!m_SoundSource.isPlaying)
        {
            m_SoundSource.clip = null;
            m_SoundSource.Stop();
            m_SoundSource.clip = clip;
            m_SoundSource.Play();
        }
        return clip.length;
    }
    public void StopAllClip()
    {
        m_SoundSource.clip = null;
        m_SoundSource.Stop();
        m_BGMSource.Stop();
    }

    public void PlaySound(AudioClip clip, Action callback)
    {
        float f = PlaySound(clip);
        StartCoroutine(waitAudioFinish(f, callback));
    }

    private IEnumerator waitAudioFinish(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
