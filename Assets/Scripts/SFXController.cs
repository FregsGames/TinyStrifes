using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    private AudioSource aSource;
    public AudioClip[] effects;

    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    public void PlayEffect(int index)
    {
        if (effects.Length - 1 < index)
            return;
        aSource.PlayOneShot(effects[index]);
    }

    public void ToggleSFX()
    {
        if (aSource.volume == 0.0f)
            aSource.volume = 0.5f;
        else
            aSource.volume = 0.0f;
    }

    //Singleton
    public static SFXController instance;
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
