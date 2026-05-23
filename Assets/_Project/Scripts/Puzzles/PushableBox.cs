using UnityEngine;

public class PushableBox : MonoBehaviour
{
    private Rigidbody2D rb;
    private float soundTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Si la caja se está moviendo a cierta velocidad
        if (rb != null && rb.linearVelocity.magnitude > 0.5f)
        {
            soundTimer -= Time.deltaTime;

            // Hacemos que suene periódicamente mientras se desliza
            if (soundTimer <= 0f)
            {
                if (SoundManager.instance != null)
                    SoundManager.instance.PlaySFX(SoundManager.instance.moveBoxClip);

                soundTimer = 0.4f; // Espera un poco antes de volver a sonar
            }
        }
        else
        {
            soundTimer = 0f; // Reinicia el temporizador si se para
        }
    }
}