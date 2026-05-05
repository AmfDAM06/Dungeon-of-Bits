using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAl : MonoBehaviour
{
    [Header("Configuración de IA")]
    public float moveSpeed = 2.5f;
    public float detectionRange = 6f; // NUEVO: Radio de visión del enemigo
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
            // Calculamos la distancia exacta entre el enemigo y el jugador
            float distanceToTarget = Vector2.Distance(rb.position, target.position);

            // Si el jugador entra en su campo de visión
            if (distanceToTarget <= detectionRange)
            {
                Vector2 direction = ((Vector2)target.position - rb.position).normalized;
                rb.linearVelocity = direction * moveSpeed;
            }
            else
            {
                // Si el jugador se aleja demasiado, el enemigo se detiene (pierde el interés)
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // NUEVO: Esta función dibuja un círculo amarillo en Unity (solo en la pestaña Scene) 
    // para que puedas ver exactamente hasta dónde llega la vista de cada enemigo.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}