using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource Music;
    public AudioSource Rebound;
    public AudioSource Explosion;
    public AudioSource Noise;

    private void Awake()
    {
        Instance = this;
    }
}
