using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    BasicMovementScript playerScript;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AudioOneShot(AudioClip audioClip)
    {
        menuAudioSource.clip = audioClip;
        menuAudioSource.PlayOneShot(audioClip);
    }

    public void Update()
    {
        heartbeatVolume = 1 - Mathf.Clamp( playerScript.enemyDistance / heartbeatThreshold, 0,1);
        heartBeatAudioSource.volume = heartbeatVolume;
    }
}
