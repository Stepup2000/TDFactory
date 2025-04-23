using UnityEngine;

/// <summary>
/// Manages an audio source component in a pooled system.
/// Handles spawning, despawning, and resetting of the audio source.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PooledAudioSource : MonoBehaviour, IPoolable
{
    private AudioSource audioSource;

    /// <summary>
    /// Initializes the audio source reference when the object is created.
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Gets the audio source component attached to the game object.
    /// </summary>
    public AudioSource AudioSource => audioSource;

    /// <summary>
    /// Activates the game object when it is spawned in the pool.
    /// </summary>
    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Resets the audio source and deactivates the game object when it is despawned.
    /// </summary>
    public void OnDespawn()
    {
        ResetObject();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets the audio source to its default state (stops playback, clears the clip, disables looping, and sets pitch to 1).
    /// </summary>
    public void ResetObject()
    {
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.pitch = 1f;
    }

    /// <summary>
    /// Plays an audio clip at a specific position with optional pitch and looping.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="position">The world position to play the sound at.</param>
    /// <param name="pitch">The pitch of the audio (default is 1).</param>
    /// <param name="loop">Whether the audio should loop (default is false).</param>
    public void Play(AudioClip clip, Vector3 position, float pitch = 1f, bool loop = false)
    {
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.loop = loop;
        audioSource.transform.position = position;
        audioSource.Play();
    }
}
