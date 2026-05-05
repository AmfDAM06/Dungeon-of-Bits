using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAl : MonoBehaviour
{
    [Header("Configuración de IA")]
    public float moveSpeed = 2.5f;
    public float detectionRange = 6f; // Rango de visión
    public Transform target;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(rb.position, target.position);

            if (distanceToTarget <= detectionRange)
            {
                Vector2 direction = ((Vector2)target.position - rb.position).normalized;
                rb.linearVelocity = direction * moveSpeed;

                // --- SISTEMA DE FLIP ---
                if (spriteRenderer != null)
                {
                    if (direction.x < 0) spriteRenderer.flipX = true;
                    else if (direction.x > 0) spriteRenderer.flipX = false;
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero; // Se detiene si te alejas
            }
        }
        else rb.linearVelocity = Vector2.zero;
    }

    // Dibuja un círculo amarillo en la pestaña "Scene" de Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}