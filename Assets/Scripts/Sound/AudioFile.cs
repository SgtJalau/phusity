using System;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AudioFile
{
    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 1;
    [Range(0.1f, 3f)] public float pitch = 1;

    public bool loop;

}