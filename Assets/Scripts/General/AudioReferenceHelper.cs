using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class AudioHelper : MonoBehaviour
{
    public  void PlaySoundAtLocation(AudioClip audioClip)
    {
        SoundManager.Instance.PlaySoundAtLocation(audioClip, Vector3.zero, false, false);
    }
}
