using System;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

[System.Serializable]
public class Sound 
{
    [FormerlySerializedAs("sound")] public SoundType soundType;
    
    public AudioFile[] audioFiles;

    [HideInInspector] public AudioSource source;
    
    public AudioFile GetRandomAudioFile()
    {
        if (audioFiles.Length == 0)
        {
            return audioFiles[0];
        }
        
        Random random = new Random();
        return audioFiles[random.Next(0, audioFiles.Length)];
    }
    
    public bool PlayRandomAudioFile()
    {
        AudioFile audioFile = GetRandomAudioFile();
        source.clip = audioFile.clip;
        source.volume = audioFile.volume;
        source.pitch = audioFile.pitch;
        source.loop = audioFile.loop;
        source.Play();

        return true;
    }
    
    public bool PlayRandomAudioFile(float pitch, float volume)
    {
        AudioFile audioFile = GetRandomAudioFile();
        source.clip = audioFile.clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = audioFile.loop;
        source.Play();

        return true;
    }
}
