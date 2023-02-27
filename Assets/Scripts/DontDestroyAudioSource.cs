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

    void Play(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayClick1() => Play(click1);
    public void PlayClick2() => Play(click2);
    public void PlayExplosion() => Play(explosion);
    public void PlayAudioStartGame() => Play(audioStartGame);
}
