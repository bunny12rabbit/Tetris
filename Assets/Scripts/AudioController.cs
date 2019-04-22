using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AudioController : MonoBehaviour
{
    [Inject] private GameConfig _config;

    private AudioSource _audioSource;
    private int _lastSong = 0;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
    }

    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = _config.music[GetRandomClipIndex()];
            _audioSource.Play();
        }
    }

    private int GetRandomClipIndex()
    {
        int randomIndex = _lastSong;
        while (randomIndex == _lastSong)
        {
            randomIndex = Random.Range(0, _config.music.Length);
        }
        _lastSong = randomIndex;

        return randomIndex;
    }

    /// <summary>
    /// Plays audio clip when figure is moved left, right or down
    /// </summary>
    public void PlayMoveAudio()
    {
        _audioSource.PlayOneShot(_config.moveSound);
    }

    /// <summary>
    /// Plays audio clip when figure is rotated
    /// </summary>
    public void PlayRotateAudio()
    {
        _audioSource.PlayOneShot(_config.moveSound);
    }

    /// <summary>
    /// Plays audio clip when figure lands
    /// </summary>
    public void PlayLandAudio()
    {
        _audioSource.PlayOneShot(_config.landSound);
    }

    /// <summary>
    /// Plays audio when line is cleared
    /// </summary>
    public void PlayClearedLineAudio()
    {
        _audioSource.PlayOneShot(_config.clearedLineSound);
    }

}
