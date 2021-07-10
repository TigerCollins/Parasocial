using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    BasicMovementScript playerScript;
    [SerializeField]
    AudioMixer audioMixer;

    [Header("UI Settings")]
    [SerializeField]
    Slider SFXSlider;
    [SerializeField]
    Slider menuSlider;

    [Header("Audio Settings")]
    [SerializeField]
    float sfxVolume;
    [SerializeField]
    float menuVolume;

    [Header("HeartBeat SFX")]
    [SerializeField]
    float heartbeatThreshold;
    [SerializeField]
    AudioSource heartBeatAudioSource;
    [SerializeField]
    float heartbeatVolume;

    [Header("Menu")]
    [SerializeField]
    AudioSource menuAudioSource;
    [SerializeField]
    AudioSource viewerCountSource;
    [SerializeField]
    AudioSource subscriberCountSource;
    [SerializeField]
    AudioSource throwSource;

    [Header("Pitcher")]
    [SerializeField]
    float minPitch;
    [SerializeField]
    float maxPitch;
    // Start is called before the first frame update
    void Awake()
    {
        if(PlayerPrefs.GetInt("AudioTriggeredCount") == 0)
        {
            PlayerPrefs.SetInt("AudioTriggeredCount", 1);
            
        }

        else
        {
            GetSFXSlider();
            GetMenuSlider();
            CheckSFXSlider();
            CheckMenuSlider();
        }
        
    }

    public void AudioOneShot(AudioClip audioClip)
    {
        menuAudioSource.clip = audioClip;
        menuAudioSource.PlayOneShot(audioClip);
    }

    public void ThrowOneShot()
    {
        throwSource.pitch = Random.Range(minPitch, maxPitch);
        throwSource.Play();
    }

    public void ViewerOneShot()
    {
        viewerCountSource.Play();
    }

    public void SubscriberOneShot()
    {
        subscriberCountSource.Play();
    }

    public void Update()
    {
        heartbeatVolume = 1 - Mathf.Clamp( playerScript.enemyDistance / heartbeatThreshold, 0,1);
        heartBeatAudioSource.volume = heartbeatVolume;
    }

    public void CheckSFXSlider()
    {
        PlayerPrefs.SetInt("SFXVolume", (int)SFXSlider.value);
        audioMixer.SetFloat("SfxParam", Mathf.Log10((int)SFXSlider.value-1) * 20);
    }

    public void GetSFXSlider()
    {
        SFXSlider.value = PlayerPrefs.GetInt("SFXVolume");
    }

    public void CheckMenuSlider()
    {
        PlayerPrefs.SetInt("MenuVolume", (int)menuSlider.value);
        
        audioMixer.SetFloat("MenuParam",  Mathf.Log10((int)menuSlider.value-1) * 20);
    }

    public void GetMenuSlider()
    {
        menuSlider.value = PlayerPrefs.GetInt("MenuVolume");
    }
}

