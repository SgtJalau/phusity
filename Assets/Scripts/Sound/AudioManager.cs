using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play(SoundType.Theme);
    }

    public Sound FindSound(SoundType soundType)
    {
        return Array.Find(sounds, a => a.soundType == soundType);
    }

    public void Play(SoundType soundType)
    {
        FindSound(soundType)?.source.Play();
    }

    public void Play(SoundType soundType, float pitch, float volume)
    {
        Sound s = FindSound(soundType);

        if (s == null)
        {
            return;
        }

        s.source.volume = volume;
        s.source.pitch = pitch;
        s.source.Play();
    }
}