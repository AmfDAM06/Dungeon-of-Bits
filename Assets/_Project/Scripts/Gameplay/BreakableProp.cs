using UnityEngine;

public class BreakableProp : MonoBehaviour
{
    public void Smash()
    {
        // Reproducimos un sonido (reciclamos el de romper/golpear)
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.hitClip);
        }

        // Destruimos el objeto
        Destroy(gameObject);
    }
}