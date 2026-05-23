using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Reproductores (AudioSource)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource loopSource;

    [Header("Música")]
    public AudioClip bgMusic;
    public AudioClip mainMenuMusic;

    [Header("Jugador (SFX)")]
    public AudioClip swordSwingClip;
    public AudioClip hitClip;
    public AudioClip playerHurtClip;
    public AudioClip shieldLoopClip;
    public AudioClip footstepClip;
    public AudioClip drinkPotionClip;

    [Header("Inventario y Botín (SFX)")]
    public AudioClip pickupItemClip;
    public AudioClip openChestClip;
    public AudioClip bombExplosionClip;

    [Header("Entorno y Puzles (SFX)")]
    public AudioClip switchToggleClip;
    public AudioClip moveBoxClip;
    public AudioClip rotateRailClip;
    public AudioClip minecartMoveClip;
    public AudioClip logicDoorClip;
    public AudioClip levelCompleteClip;

    [Header("Hacking Terminal (SFX)")]
    public AudioClip terminalStartClip;
    public AudioClip terminalSuccessClip;
    public AudioClip terminalErrorClip;

    [Header("Enemigos (SFX)")]
    public AudioClip enemyHurtClip;
    public AudioClip arrowShootClip;
    public AudioClip bossDeathClip;

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