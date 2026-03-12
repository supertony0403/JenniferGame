using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip mainMenuMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip interactSFX;
    [SerializeField] private AudioClip purchaseSFX;
    [SerializeField] private AudioClip ausrasterSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        WutMeter.Instance?.OnAusraster.AddListener(PlayAusrasterSFX);
        if (mainMenuMusic != null) PlayMusic(mainMenuMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayInteract() => sfxSource?.PlayOneShot(interactSFX);
    public void PlayPurchase() => sfxSource?.PlayOneShot(purchaseSFX);
    private void PlayAusrasterSFX() => sfxSource?.PlayOneShot(ausrasterSFX);
}
