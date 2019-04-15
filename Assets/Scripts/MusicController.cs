using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance = null;  //  Instance of an object

    public AudioClip[] music;  
    public AudioClip mainTheme;

    private AudioSource _audioSource;
    private int _lastSong = 0;

    #region Singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
    }

    int GetRandomClip()
    {
        int randomIndex = _lastSong;
        while (randomIndex == _lastSong)
        {
            randomIndex = Random.Range(0, music.Length);
        }
        _lastSong = randomIndex;

        return randomIndex;
    }

    void Update()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = music[GetRandomClip()];
            _audioSource.Play();
        }
    }
}
