using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] AudioClip[] gunShotSounds;
    [SerializeField] AudioClip jumpSound;

    public void PlayJumpSound(AudioSource auSource)
    {
        auSource.PlayOneShot(jumpSound);
    }

    public void PlayGunShotSound(AudioSource auSource)
    {
        var rand = Random.Range(0, gunShotSounds.Length);
        auSource.PlayOneShot(gunShotSounds[rand]);
    }
}
