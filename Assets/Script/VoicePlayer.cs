using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    public AudioClip voice;
    private AudioSource univoice;

    // Use this for initialization
    void Start()
    {
        univoice = GetComponent<AudioSource>();
        univoice.clip = voice;
        univoice.Play();
    }
}