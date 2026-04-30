using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Configuración de IA")]
    public float moveSpeed = 2.5f;
    public Transform target;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // Calculamos la dirección
            Vector2 direction = ((Vector2)target.position - rb.position).normalized;

            // Aplicamos la velocidad lineal (API moderna de Unity)
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}