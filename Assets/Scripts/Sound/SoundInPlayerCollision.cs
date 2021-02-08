using UnityEngine;


public class SoundInPlayerCollision : MonoBehaviour
{
    
    public SoundType soundType;

    private Sound _playingSound = null;

    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && _playingSound == null )
        {
            _playingSound = _audioManager.Play(soundType);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && _playingSound != null && _playingSound.source.isPlaying)
        {
            _playingSound.source.Stop();
            _playingSound = null;
        }
    }
}