using System.Collections.Generic;
using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFXHandler : MonoBehaviour
{
    [SerializeField] private List<AudioClip> m_generators;
    
    [SerializeField] private Vector2 m_minMaxPitch;
    [SerializeField] private List<AudioSource> m_audioSource;
    [SerializeField] private bool m_playOnAwake;
    [SerializeField] private float m_fadeTime;

    Coroutine m_fadeRoutine;

    private void OnEnable()
    {
        GameEvents.Instance.onGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onGameOver -= OnGameOver;
    }

    private void Awake()
    {
        if (m_playOnAwake)
        {
            PlaySound();
        }
    }

    private void OnGameOver()
    {
        if(!m_playOnAwake) return;

        StopAllCoroutines();
        
        foreach(AudioSource source in m_audioSource)
        {
            source.volume = 0f;
        }
    }

    public void PlaySound()
    {
        AudioSource source = m_audioSource[0];

        if (source.isPlaying && m_audioSource.Count > 1)
        {
            source = m_audioSource[1];  
        }

        int index = Random.Range(0, m_generators.Count);
        AudioClip generator = m_generators[index];

        source.generator = generator;
        source.pitch = Random.Range(m_minMaxPitch.x, m_minMaxPitch.y);

        source.Play();
    }

    public void FadeIn()
    {
        AudioSource source = m_audioSource[0];

        if(m_fadeRoutine != null)
        {
            StopCoroutine(m_fadeRoutine);    
        }

        m_fadeRoutine = StartCoroutine(FadeAudioRoutine(source, 1f));
    }
    public void FadeOut()
    {
        AudioSource source = m_audioSource[0];

        if(m_fadeRoutine != null)
        {
            StopCoroutine(m_fadeRoutine);    
        }

        m_fadeRoutine = StartCoroutine(FadeAudioRoutine(source, 0f));
    }

    private IEnumerator FadeAudioRoutine(AudioSource source, float target)
    {
        while(source.volume != target)
        {
            float currentVolume = source.volume;
            yield return null;
            source.volume = Mathf.Lerp(currentVolume, target, Time.deltaTime * m_fadeTime);
        }
    }

}
