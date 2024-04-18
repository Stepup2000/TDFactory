using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();
                if (_instance == null)
                {
                    GameObject soundManagerObject = new GameObject("SoundManager");
                    _instance = soundManagerObject.AddComponent<SoundManager>();
                }
            }
            return _instance;
        }
    }
    private static SoundManager _instance;

    // AudioClip variables
    [SerializeField] private AudioClip _buttonSoundClip;
    [SerializeField] private AudioClip _swapSoundClip;
    [SerializeField] private AudioClip _saveSoundClip;
    [SerializeField] private float soundCooldown = 0.15f;

    // Object pooling variables
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxPoolSize = 20;

    private List<AudioSource> audioSourcesPool = new List<AudioSource>();
    private GameObject _audioSourceContainer;

    // Dictionary to store last played times of each sound
    private Dictionary<AudioClip, float> lastPlayTimes = new Dictionary<AudioClip, float>();
    // Time window in which the same sound won't be played again
    

    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate SoundManager instance found. Destroying this instance.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private GameObject GetAudioSourceContainer()
    {
        if (_audioSourceContainer == null)
        {
            _audioSourceContainer = new GameObject("AudioSourceContainer");
            _audioSourceContainer.transform.SetParent(_instance.transform);
        }
        return _audioSourceContainer;
    }

    public void PlaySoundAtLocation(AudioClip clip, Vector3 position, bool randomizePitch = false, bool loop = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("No audioclip was given");
            return;
        }

        // Check if the sound is on cooldown
        if (lastPlayTimes.ContainsKey(clip) && Time.time - lastPlayTimes[clip] < soundCooldown)
        {
            return; // Sound is still on cooldown
        }

        AudioSource audioSource = GetAvailableAudioSource();
        if (audioSource == null)
        {
            Debug.LogWarning("No available AudioSource in the pool. Increasing pool size.");
            CreateNewAudioSource();
            audioSource = GetAvailableAudioSource(); // Try again after increasing pool size
            if (audioSource == null)
            {
                Debug.LogWarning("Unable to play sound. AudioSource pool is full and limit has been reached.");
                return;
            }
        }

        audioSource.gameObject.SetActive(true);
        if (clip != null)
        {
            audioSource.transform.position = position;
            audioSource.clip = clip;
            audioSource.Play();

            if (randomizePitch == true) audioSource.pitch = GetRandomNumber(0.9f, 1.1f);
            if (loop == true) audioSource.loop = true;
            else StartCoroutine(ReturnToPool(audioSource, clip.length));

            // Update last play time
            lastPlayTimes[clip] = Time.time;
        }
        else
        {
            Debug.LogWarning("Sound clip not found: " + clip.name);
        }
    }

    private AudioClip LoadSound(string soundName)
    {
        // Load sound from resources
        return Resources.Load<AudioClip>("Sounds/" + soundName);
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSourcesPool)
        {
            if (!source.gameObject.activeSelf)
                return source;
        }
        return null;
    }

    private void CreateNewAudioSource()
    {
        if (audioSourcesPool.Count >= maxPoolSize)
            return;

        GameObject newAudioSource = new GameObject("AudioSource");
        AudioSource audioSource = newAudioSource.AddComponent<AudioSource>();
        audioSourcesPool.Add(audioSource);
        newAudioSource.transform.SetParent(GetAudioSourceContainer().transform);
        newAudioSource.gameObject.SetActive(false);
    }

    private IEnumerator ReturnToPool(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.pitch = 1;
        audioSource.gameObject.SetActive(false);
    }

    private float GetRandomNumber(float min, float max)
    {
        return Random.Range(min, max);
    }

    public void PlayButtonSound()
    {
        PlaySoundAtLocation(_buttonSoundClip, transform.position, true);
    }

    public void PlaySwapSound()
    {
        PlaySoundAtLocation(_swapSoundClip, transform.position, true);
    }

    public void PlaySaveSound()
    {
        PlaySoundAtLocation(_saveSoundClip, transform.position, true);
    }
}
