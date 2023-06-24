using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioSource musicSource;
    // Start is called before the first frame update
    
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
}
