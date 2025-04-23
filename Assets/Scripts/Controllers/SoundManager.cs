using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages audio playback in the game, including background music and sound effects.
/// Uses an object pool to manage pooled audio sources for efficient sound management.
/// </summary>
public class SoundManager : BaseObjectPooler<PooledAudioSource>
{
    private static SoundManager _instance;

    /// <summary>
    /// Singleton instance of the SoundManager. Ensures only one instance exists.
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
                    GameObject go = new GameObject("SoundManager");
                    _instance = go.AddComponent<SoundManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private float soundCooldown = 0.15f;

    private readonly Dictionary<AudioClip, float> lastPlayTimes = new();

    /// <summary>
    /// Initializes the SoundManager, ensuring only one instance exists and starts background music.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        StartBackgroundMusic();
    }

    /// <summary>
    /// Plays a sound at a specific location with optional pitch randomization and looping.
    /// Ensures sounds are not played too frequently by checking the cooldown.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="position">The world position to play the sound at.</param>
    /// <param name="randomizePitch">Whether to randomize the pitch between 0.9 and 1.1.</param>
    /// <param name="loop">Whether the sound should loop.</param>
    public void PlaySoundAtLocation(AudioClip clip, Vector3 position, bool randomizePitch = false, bool loop = false)
    {
        if (clip == null)
        {
            Debug.LogWarning("No audioclip was given.");
            return;
        }

        if (lastPlayTimes.TryGetValue(clip, out float lastTime) && Time.time - lastTime < soundCooldown)
            return;

        var audioSource = GetFromPool();
        float pitch = randomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
        audioSource.Play(clip, position, pitch, loop);

        if (!loop)
            StartCoroutine(ReturnToPoolAfterDelay(audioSource, clip.length));

        lastPlayTimes[clip] = Time.time;
    }

    /// <summary>
    /// Returns the audio source to the pool after a specified delay.
    /// </summary>
    /// <param name="audioSource">The audio source to return to the pool.</param>
    /// <param name="delay">The delay before returning the audio source.</param>
    private IEnumerator ReturnToPoolAfterDelay(PooledAudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(audioSource);
    }

    /// <summary>
    /// Starts playing background music on a loop when the SoundManager is initialized.
    /// </summary>
    private void StartBackgroundMusic()
    {
        PlaySoundAtLocation(_backgroundMusic, transform.position, false, true);
    }
}
