using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 5f;
    public int damage = 1;
    public float lifetime = 3f; // Se destruye tras 3 segundos para no saturar la memoria

    void Start()
    {
        // Programamos su autodestrucción por si falla y sale volando al infinito
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // En Unity, Vector3.right representa el "Frente" cuando rotamos un objeto en 2D
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si choca con el jugador
        if (collision.CompareTag("Player"))
        {
            // Intentamos hacerle daño
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // La flecha desaparece al impactar
        }
        // Si choca con una pared o un obstáculo (asegúrate de que tus muros tengan la tag "Wall" o similar)
        else if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
        // IMPORTANTE: Si el jugador levanta el escudo y el escudo tiene su propio collider con la tag "Shield", la flecha rebotará o se destruirá sin hacer daño a la vida.
        else if (collision.CompareTag("Shield"))
        {
            Debug.Log("¡Flecha bloqueada por el escudo!");
            Destroy(gameObject);
        }
    }
}