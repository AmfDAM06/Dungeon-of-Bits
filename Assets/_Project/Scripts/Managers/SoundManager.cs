using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Reproductores (AudioSource)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource loopSource; // <-- NUEVO: Para sonidos continuos (como el escudo o la vagoneta)

    [Header("Música")]
    public AudioClip bgMusic;
    public AudioClip mainMenuMusic; // <-- NUEVO

    [Header("Jugador (SFX)")]
    public AudioClip swordSwingClip;
    public AudioClip hitClip;
    public AudioClip playerHurtClip;
    public AudioClip shieldLoopClip; // <-- NUEVO
    public AudioClip footstepClip;   // <-- NUEVO
    public AudioClip drinkPotionClip; // <-- NUEVO

    [Header("Inventario y Botín (SFX)")]
    public AudioClip pickupItemClip; // <-- NUEVO
    public AudioClip openChestClip;  // <-- NUEVO
    public AudioClip bombExplosionClip; // <-- NUEVO

    [Header("Entorno y Puzles (SFX)")]
    public AudioClip switchToggleClip; // <-- NUEVO (Para palancas y placas)
    public AudioClip moveBoxClip;      // <-- NUEVO
    public AudioClip rotateRailClip;   // <-- NUEVO
    public AudioClip minecartMoveClip; // <-- NUEVO
    public AudioClip logicDoorClip;    // <-- NUEVO
    public AudioClip levelCompleteClip; // <-- NUEVO

    [Header("Hacking Terminal (SFX)")]
    public AudioClip terminalStartClip;   // <-- NUEVO
    public AudioClip terminalSuccessClip; // <-- NUEVO
    public AudioClip terminalErrorClip;   // <-- NUEVO

    [Header("Enemigos (SFX)")]
    public AudioClip enemyHurtClip;
    public AudioClip arrowShootClip;
    public AudioClip bossDeathClip; // <-- NUEVO

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMusic(bgMusic);
    }

    // Reproduce música de fondo
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null && musicSource != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // Reproduce un efecto corto
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Inicia un sonido en bucle (para el escudo, vagoneta...)
    public void PlayLoop(AudioClip clip)
    {
        if (clip != null && loopSource != null && loopSource.clip != clip)
        {
            loopSource.clip = clip;
            loopSource.loop = true;
            loopSource.Play();
        }
    }

    // Detiene el sonido en bucle
    public void StopLoop()
    {
        if (loopSource != null)
        {
            loopSource.Stop();
            loopSource.clip = null;
        }
    }
}