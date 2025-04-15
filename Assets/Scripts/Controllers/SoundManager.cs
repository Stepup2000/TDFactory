using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages and plays sound effects within the game. Implements a singleton pattern to ensure only one instance exists.
/// </summary>
public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    /// <summary>
    /// Gets the singleton instance of the SoundManager. Creates one if it does not exist.
    /// </summary>
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();
                if (_instance == null)
                {
                    // Create a new GameObject with SoundManager component if none exists
                    GameObject soundManagerObject = new GameObject("SoundManager");
                    _instance = soundManagerObject.AddComponent<SoundManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private AudioClip _buttonSoundClip;
    [SerializeField] private AudioClip _swapSoundClip;
    [SerializeField] private AudioClip _saveSoundClip;
    [SerializeField] private float soundCooldown = 0.15f;

    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxPoolSize = 20; 

    private List<AudioSource> audioSourcesPool = new List<AudioSource>();
    private GameObject _audioSourceContainer;

    private Dictionary<AudioClip, float> lastPlayTimes = new Dictionary<AudioClip, float>();

    /// <summary>
    /// Called when the script is enabled. Sets up the singleton instance and initializes the audio source pool.
    /// </summary>
    private void Awake()
    {
        // Ensure this instance is the only one, destroy duplicates
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate SoundManager instance found. Destroying this instance.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        StartBackgroundMusic();
        InitializePool();
    }

    /// <summary>
    /// Initializes the audio source pool with a predefined number of sources.
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    /// <summary>
    /// Retrieves or creates the container for audio source objects.
    /// </summary>
    /// <returns>The audio source container GameObject.</returns>
    private GameObject GetAudioSourceContainer()
    {
        if (_audioSourceContainer == null)
        {
            // Create a new container if it does not exist
            _audioSourceContainer = new GameObject("AudioSourceContainer");
            _audioSourceContainer.transform.SetParent(_instance.transform);
        }
        return _audioSourceContainer;
    }

    /// <summary>
    /// Plays a sound at a specified location.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    /// <param name="position">The position to play the sound at.</param>
    /// <param name="randomizePitch">Whether to randomize the pitch of the sound.</param>
    /// <param name="loop">Whether the sound should loop.</param>
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
            return;
        }

        AudioSource audioSource = GetAvailableAudioSource();
        if (audioSource == null)
        {
            Debug.LogWarning("No available AudioSource in the pool. Increasing pool size.");
            CreateNewAudioSource(); // Increase pool size if necessary
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

            if (randomizePitch)
                audioSource.pitch = GetRandomNumber(0.9f, 1.1f);
            if (loop)
                audioSource.loop = true;
            else
                StartCoroutine(ReturnToPool(audioSource, clip.length)); // Return audio source to pool after sound is done

            // Update last play time
            lastPlayTimes[clip] = Time.time;
        }
        else
        {
            Debug.LogWarning("Sound clip not found: " + clip.name);
        }
    }

    /// <summary>
    /// Loads an AudioClip from resources based on its name.
    /// </summary>
    /// <param name="soundName">The name of the sound to load.</param>
    /// <returns>The loaded AudioClip.</returns>
    private AudioClip LoadSound(string soundName)
    {
        return Resources.Load<AudioClip>("Sounds/" + soundName);
    }

    /// <summary>
    /// Retrieves an available AudioSource from the pool.
    /// </summary>
    /// <returns>An available AudioSource, or null if none are available.</returns>
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSourcesPool)
        {
            if (!source.gameObject.activeSelf)
                return source;
        }
        return null;
    }

    /// <summary>
    /// Creates a new AudioSource and adds it to the pool, provided the pool size limit has not been reached.
    /// </summary>
    private void CreateNewAudioSource()
    {
        if (audioSourcesPool.Count >= maxPoolSize)
            return; // Do not exceed the maximum pool size

        GameObject newAudioSource = new GameObject("AudioSource");
        AudioSource audioSource = newAudioSource.AddComponent<AudioSource>();
        audioSourcesPool.Add(audioSource);
        newAudioSource.transform.SetParent(GetAudioSourceContainer().transform);
        newAudioSource.gameObject.SetActive(false); // Initially inactive
    }

    /// <summary>
    /// Coroutine that handles returning an AudioSource to the pool after a delay.
    /// </summary>
    /// <param name="audioSource">The AudioSource to return to the pool.</param>
    /// <param name="delay">The delay before returning to the pool.</param>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator ReturnToPool(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.pitch = 1;
        audioSource.gameObject.SetActive(false); // Return audio source to pool
    }

    /// <summary>
    /// Returns a random float value between the specified minimum and maximum values.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>A random float value between min and max.</returns>
    private float GetRandomNumber(float min, float max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// Plays the background music.
    /// </summary>
    private void StartBackgroundMusic()
    {
        PlaySoundAtLocation(_backgroundMusic, transform.position, false, true);
    }
}
