using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject swordObject;
    public Animator animator;

    [Header("Estadísticas de Combate")]
    public float attackDuration = 0.15f;
    public float attackCooldown = 0.4f;

    [Header("Posiciones del Hitbox (NUEVO)")]
    // Aquí puedes ajustar desde el Inspector dónde aparece el golpe y cómo rota
    public Vector3 sideHitboxOffset = new Vector3(1f, 0f, 0f);
    public Vector3 upHitboxOffset = new Vector3(0f, 1f, 0f);
    public Vector3 downHitboxOffset = new Vector3(0f, -1f, 0f);

    public Vector3 sideHitboxRotation = new Vector3(0f, 0f, 0f);
    public Vector3 upHitboxRotation = new Vector3(0f, 0f, 90f);
    public Vector3 downHitboxRotation = new Vector3(0f, 0f, -90f);

    private bool isAttacking = false;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (animator != null)
        {
            // Disparamos la animación visual
            animator.SetTrigger("Attack");

            // --- NUEVO LÓGICA DE HITBOX ---
            // Leemos hacia dónde estaba mirando el Animator
            float moveY = animator.GetFloat("MoveY");

            // Si miramos hacia Arriba
            if (moveY > 0.1f)
            {
                swordObject.transform.localPosition = upHitboxOffset;
                swordObject.transform.localRotation = Quaternion.Euler(upHitboxRotation);
            }
            // Si miramos hacia Abajo
            else if (moveY < -0.1f)
            {
                swordObject.transform.localPosition = downHitboxOffset;
                swordObject.transform.localRotation = Quaternion.Euler(downHitboxRotation);
            }
            // Si no es arriba ni abajo, es a los Lados (el Flip de escala ya se encarga de la Izquierda)
            else
            {
                swordObject.transform.localPosition = sideHitboxOffset;
                swordObject.transform.localRotation = Quaternion.Euler(sideHitboxRotation);
            }
        }

        // 1. Activamos la espada (Hitbox)
        swordObject.SetActive(true);

        // 2. Esperamos lo que dura el "tajo"
        yield return new WaitForSeconds(attackDuration);

        // 3. Desactivamos la espada
        swordObject.SetActive(false);

        // 4. Esperamos el tiempo de recarga
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }
}