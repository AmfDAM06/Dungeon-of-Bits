using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Hacemos que sea un Singleton para poder llamarlo desde cualquier parte
    public static SoundManager Instance;

    [Header("Reproductores (AudioSources)")]
    public AudioSource bgmSource; // Para la música de fondo
    public AudioSource sfxSource; // Para los efectos de sonido

    [Header("Efectos de Sonido (Huecos vacíos por ahora)")]
    public AudioClip swordSwing;
    public AudioClip chestOpen;
    public AudioClip terminalError;
    public AudioClip terminalSuccess;
    public AudioClip playerHurt;

    [Header("Música")]
    public AudioClip dungeonMusic;

    private void Awake()
    {
        // Configuramos el Singleton para que sobreviva al cambiar de escena
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ¡Magia! Este objeto no morirá al cambiar de piso
        }
        else
        {
            Destroy(gameObject); // Evita que haya dos SoundManagers a la vez
            return;
        }
    }

    private void Start()
    {
        // Reproducir música de fondo al empezar si hay alguna asignada
        if (dungeonMusic != null)
        {
            PlayBGM(dungeonMusic);
        }
    }

    // --- FUNCIONES PARA REPRODUCIR SONIDOS ---

    // Reproduce la música en bucle
    public void PlayBGM(AudioClip musicClip)
    {
        if (bgmSource == null || musicClip == null) return;

        bgmSource.clip = musicClip;
        bgmSource.loop = true; // La música debe repetirse
        bgmSource.Play();
    }

    // Reproduce un efecto especial (permite que varios suenen a la vez sin cortarse)
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;

        sfxSource.PlayOneShot(clip);
    }
}