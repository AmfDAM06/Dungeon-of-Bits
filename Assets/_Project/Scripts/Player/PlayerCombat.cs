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

    [Header("Posiciones del Hitbox")]
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

        // --- NUEVO: SONIDO DE TAJO AL AIRE ---
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.swordSwingClip);
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");

            float moveY = animator.GetFloat("MoveY");

            if (moveY > 0.1f)
            {
                swordObject.transform.localPosition = upHitboxOffset;
                swordObject.transform.localRotation = Quaternion.Euler(upHitboxRotation);
            }
            else if (moveY < -0.1f)
            {
                swordObject.transform.localPosition = downHitboxOffset;
                swordObject.transform.localRotation = Quaternion.Euler(downHitboxRotation);
            }
            else
            {
                swordObject.transform.localPosition = sideHitboxOffset;
                swordObject.transform.localRotation = Quaternion.Euler(sideHitboxRotation);
            }
        }

        swordObject.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        swordObject.SetActive(false);
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }
}