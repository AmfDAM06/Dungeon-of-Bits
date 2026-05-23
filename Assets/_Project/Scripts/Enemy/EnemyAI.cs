using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Configuración de IA")]
    public float moveSpeed = 2.5f;
    public float detectionRange = 6f; // Rango para empezar a perseguir
    public float attackRange = 1.2f;  // Rango para pararse y pegar
    public float attackCooldown = 1.5f; // Tiempo entre ataques

    [Header("Daño")]
    public int attackDamage = 1;

    public Transform target;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Buscamos los componentes visuales
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        // --- NUEVO: SI NO HAY JUGADOR, LO SEGUIMOS BUSCANDO ---
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
            else
            {
                // Si sigue sin haber jugador, nos quedamos quietos
                rb.linearVelocity = Vector2.zero;
                if (animator != null) animator.SetFloat("Speed", 0f);
                return;
            }
        }

        // Si está en medio de un ataque, no nos movemos
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetFloat("Speed", 0f);
            return;
        }
        // --- NUEVO: SI EL JUGADOR ES INVISIBLE, NOS QUEDAMOS QUIETOS ---
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null && targetHealth.isInvisible)
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetFloat("Speed", 0f);
            return; // Cortamos el código aquí para que no te persiga
        }

        float distanceToTarget = Vector2.Distance(rb.position, target.position);

        // --- ESTADO 1: ATACAR ---
        if (distanceToTarget <= attackRange)
        {
            rb.linearVelocity = Vector2.zero; // Frenamos para atacar
            if (animator != null) animator.SetFloat("Speed", 0f);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(AttackRoutine());
            }
        }
        // --- ESTADO 2: PERSEGUIR ---
        else if (distanceToTarget <= detectionRange)
        {
            Vector2 direction = ((Vector2)target.position - rb.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            // Le decimos al Animator que estamos caminando
            if (animator != null) animator.SetFloat("Speed", rb.linearVelocity.magnitude);

            // Giramos el Sprite
            if (spriteRenderer != null)
            {
                if (direction.x < 0) spriteRenderer.flipX = true;
                else if (direction.x > 0) spriteRenderer.flipX = false;
            }
        }
        // --- ESTADO 3: IDLE (Reposo) ---
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetFloat("Speed", 0f);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Disparamos la animación de ataque
        if (animator != null) animator.SetTrigger("Attack");

        // Esperamos un poquito para que el daño coincida con el movimiento de la espada/golpe
        yield return new WaitForSeconds(0.3f);

        // Comprobamos si el jugador sigue a tiro o si ha esquivado
        if (target != null && Vector2.Distance(rb.position, target.position) <= attackRange + 0.5f)
        {
            Health playerHealth = target.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        // Esperamos a que termine la animación antes de volver a perseguir
        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
    }

    // Dibuja los rangos visuales en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}