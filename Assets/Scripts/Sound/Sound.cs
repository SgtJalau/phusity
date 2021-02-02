using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Sound 
{
    [FormerlySerializedAs("sound")] public SoundType soundType;
    public AudioClip clip;

    [Range(0f,1f)]
    public float volume;
    [Range(0.1f,3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
