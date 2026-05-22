using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Esta línea crea el "Singleton" (instancia global accesible desde cualquier script)
    public static SoundManager instance;

    [Header("Reproductores (AudioSource)")]
    public AudioSource musicSource; // Para la música de fondo
    public AudioSource sfxSource;   // Para los efectos de sonido cortos

    [Header("Efectos de Sonido (SFX)")]
    public AudioClip swordSwingClip;
    public AudioClip hitClip;
    public AudioClip playerHurtClip;
    public AudioClip enemyHurtClip;
    public AudioClip arrowShootClip;

    [Header("Música")]
    public AudioClip bgMusic;

    void Awake()
    {
        // Si no hay ningún SoundManager, este se convierte en el oficial.
        // Si ya hay uno (por ejemplo, al cambiar de nivel), destruimos la copia para que la música no se reinicie ni se duplique.
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
        // Al arrancar, reproducimos la música en bucle
        if (bgMusic != null && musicSource != null)
        {
            musicSource.clip = bgMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // Esta es la función mágica que llamaremos desde otros scripts
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            // PlayOneShot permite que suenen varios efectos a la vez sin cortarse entre ellos
            sfxSource.PlayOneShot(clip);
        }
    }
}