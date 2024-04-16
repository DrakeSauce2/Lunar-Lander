using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource deathSoundAudioSource;
    [SerializeField] AudioSource thrustSoundAudioSource;
    [SerializeField] AudioSource lowFuelSoundAudioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void PlayDeathSound()
    {
        if (deathSoundAudioSource.isPlaying) return;

        deathSoundAudioSource.Play();
    }

    public void PlayThrustSound()
    {
        if (thrustSoundAudioSource.isPlaying) return;

        thrustSoundAudioSource.Play();
    }

    public void StopThrustSound()
    {
        thrustSoundAudioSource.Stop();
    }

    public void PlayLowFuelSound()
    {
        if (lowFuelSoundAudioSource.isPlaying) return;

        lowFuelSoundAudioSource.Play();
    }

    public void StopLowFuelSound()
    {
        lowFuelSoundAudioSource.Stop();
    }


}
