using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Configuraci�n de IA")]
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
            // 1. Calculamos la direcci�n hacia el jugador
            Vector2 direction = ((Vector2)target.position - rb.position).normalized;

            // 2. Usamos la velocidad del motor de f�sicas en lugar del MovePosition
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            // Si no hay objetivo (por ejemplo, si mueres), se queda quieto
            rb.linearVelocity = Vector2.zero;
        }
    }
}