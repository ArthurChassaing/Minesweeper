using UnityEngine;

public class DontDestroyAudioSource : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip click1;
    public AudioClip click2;
    public AudioClip explosion;
    public AudioClip victory;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Play(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayClick1() => Play(click1);
    public void PlayClick2() => Play(click2);
    public void PlayExplosion() => Play(explosion);
    public void PlayVictory() => Play(victory);
}
