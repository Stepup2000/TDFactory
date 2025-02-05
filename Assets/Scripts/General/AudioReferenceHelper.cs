using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper script to access the SoundManager with editor events.
/// </summary>
public class AudioHelper : MonoBehaviour
{
    /// <summary>
    /// Manages playing sounds using the SoundManager.
    /// </summary>
    public void PlaySoundAtLocation(AudioClip audioClip)
    {
        SoundManager.Instance.PlaySoundAtLocation(audioClip, Vector3.zero, false, false);
    }
}
