using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : Singleton<MusicScript>
{
    public AudioClip battleMusic;
    public AudioClip calmMusic;
    public AudioClip menuMusic;
    private AudioSource source;


    void Start()
    {
        DontDestroyOnLoad(gameObject);
        source = GetComponent<AudioSource>();
    }

    public void StartBattleMusic()
    {
        source.Stop();
        source.clip = battleMusic;
        source.Play();
    }

    public void PlayDefaultMusic()
    {
        source.Stop();
        source.clip = calmMusic;
        source.Play();
    }

    public void PlayMenuMusic()
    {
        source.Stop();
        source.clip = menuMusic;
        source.Play();
    }

    public void ChangeVolume(float volume)
    {
        source.volume = volume;
    }
}
