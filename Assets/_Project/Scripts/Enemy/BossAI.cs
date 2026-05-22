using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BossAI : MonoBehaviour
{
    [Header("Configuración de Boss")]
    public float moveSpeed = 3.5f;
    public float detectionRange = 15f;
    public float attack1Range = 1.2f;
    public float attack2Range = 3.0f;
    public float attackCooldown = 1.5f;

    [Header("Daño")]
    public int attack1Damage = 1;
    public int attack2Damage = 2;

    [Header("Efectos Visuales")]
    public GameObject specialAttackParticles; // <-- NUEVO: Aquí pondremos el Prefab del polvo

    public Transform target;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
            else return;
        }

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToTarget = Vector2.Distance(rb.position, target.position);
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;

        if (animator != null)
        {
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
        }

        // --- LÓGICA DE ATAQUES ---
        if (distanceToTarget <= attack1Range && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine(1, attack1Range, attack1Damage));
        }
        else if (distanceToTarget <= attack2Range && distanceToTarget > attack1Range && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine(2, attack2Range, attack2Damage));
        }
        // --- PERSECUCIÓN ---
        else if (distanceToTarget <= detectionRange)
        {
            rb.linearVelocity = direction * moveSpeed;
            if (animator != null) animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetFloat("Speed", 0f);
        }
    }

    private IEnumerator AttackRoutine(int attackType, float range, int damage)
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetFloat("Speed", 0f);

        if (animator != null) animator.SetTrigger(attackType == 1 ? "Attack1" : "Attack2");

        // --- NUEVO: Disparamos las partículas del ataque de área ---
        if (attackType == 2 && specialAttackParticles != null)
        {
            Instantiate(specialAttackParticles, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.3f);

        if (target != null && Vector2.Distance(rb.position, target.position) <= range + 0.5f)
        {
            Health playerHealth = target.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.6f);
        isAttacking = false;
    }
}