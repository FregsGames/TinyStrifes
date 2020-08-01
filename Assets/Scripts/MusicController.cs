using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip[] music;
    private AudioSource aSource;

    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!aSource.isPlaying && aSource != null)
            PlayRandomMusic();
    }

    public void ToggleMusic()
    {
        if (aSource.volume == 0.0f)
            aSource.volume = 0.5f;
        else
            aSource.volume = 0.0f;
    }

    void PlayRandomMusic()
    {
        if(music.Length > 0)
            aSource.clip = music[Random.Range(0, music.Length)];
        aSource.Play();
    }

    //Singleton
    public static MusicController instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
