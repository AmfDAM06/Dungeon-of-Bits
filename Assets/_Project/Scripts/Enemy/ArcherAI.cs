using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class ArcherAI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Distancias (La Inteligencia)")]
    public float sightRange = 8f;
    public float fleeRange = 4f;
    public float meleeRange = 1.5f;

    [Header("Movimiento")]
    public float fleeSpeed = 3f;

    [Header("Ataque a Distancia")]
    public float fireRate = 2f;
    private float nextFireTime = 0f;

    [Header("Ataque Cuerpo a Cuerpo")]
    public int meleeDamage = 1;
    public float meleeAttackRate = 1.5f;
    private float nextMeleeTime = 0f;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isAttacking = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null)
            {
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isWalking", false);
                return;
            }
        }

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (player.position.x < transform.position.x) spriteRenderer.flipX = true;
        else spriteRenderer.flipX = false;

        if (distance <= meleeRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);

            if (Time.time >= nextMeleeTime)
            {
                StartCoroutine(MeleeAttackRoutine());
            }
        }
        else if (distance <= fleeRange)
        {
            Vector2 fleeDirection = (transform.position - player.position).normalized;
            rb.linearVelocity = fleeDirection * fleeSpeed;
            anim.SetBool("isWalking", true);
        }
        else if (distance <= sightRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);

            if (Time.time >= nextFireTime)
            {
                StartRangedAttack();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isWalking", false);
        }
    }

    // 1. Empieza el ataque, bloquea el movimiento y dispara la animación
    private void StartRangedAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Shoot");

        // El disparo real ahora lo hará el Animation Event llamando a FireArrow()
    }

    // 2. NUEVO: Esta función la llamará la animación EXACTAMENTE cuando queramos
    public void FireArrow()
    {
        if (player != null && projectilePrefab != null && firePoint != null)
        {
            Vector2 direction = player.position - firePoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        }

        nextFireTime = Time.time + fireRate;
    }

    // 3. NUEVO: Esta función la llamará la animación cuando TERMINE, para dejarle moverse de nuevo
    public void FinishRangedAttack()
    {
        isAttacking = false;
    }

    private IEnumerator MeleeAttackRoutine()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Melee");

        yield return new WaitForSeconds(0.2f);

        if (player != null && Vector2.Distance(transform.position, player.position) <= meleeRange + 0.5f)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeDamage);
                Debug.Log("¡El arquero te ha acuchillado!");
            }
        }

        nextMeleeTime = Time.time + meleeAttackRate;
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }
}