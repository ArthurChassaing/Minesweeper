using UnityEngine;

public class DontDestroyAudioSource : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip click1;
    public AudioClip click2;
    public AudioClip explosion;
    public AudioClip audioStartGame;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
        click1 = Resources.Load<AudioClip>("Audio/Click1");
        click2 = Resources.Load<AudioClip>("Audio/Click2");
        explosion = Resources.Load<AudioClip>("Audio/Explosion");
        audioStartGame = Resources.Load<AudioClip>("Audio/Victory");
    }

    /// <summary>
    /// Play a sound effect.
    /// </summary>
    /// <param name="clip">Clip to play</param>
    void Play(AudioClip clip) => audioSource.PlayOneShot(clip);

    public void PlayClick1() => Play(click1);
    public void PlayClick2() => Play(click2);
    public void PlayExplosion() => Play(explosion);
    public void PlayAudioStartGame() => Play(audioStartGame);

    /// <summary>
    /// Change the volume of the audio source.
    /// </summary>
    /// <param name="value">New value (between 0 and 1)</param>
    public void ChangeVolume(float value)
    {
        audioSource.volume = value;
        PlayerPrefs.SetFloat("volume", value);
    }
}
