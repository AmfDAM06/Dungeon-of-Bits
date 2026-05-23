using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración")]
    public float moveSpeed = 5f;
    public float stepInterval = 0.35f; // NUEVO: Cada cuánto suena un paso

    [Header("Animación por Código")]
    public Transform visualTransform;

    private Rigidbody2D rb;
    private Vector2 movement;
    private float facingDir = 1f;

    private Animator animator;
    private Vector2 lastMovement = new Vector2(0, -1);

    private Health health;
    private float stepTimer = 0f; // NUEVO: Reloj interno para los pasos

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();

        if (visualTransform == null) visualTransform = transform;

        animator = visualTransform.GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (movement.x < 0) facingDir = -1f;
        else if (movement.x > 0) facingDir = 1f;

        if (movement.magnitude > 0)
        {
            lastMovement = movement;
        }

        if (animator != null)
        {
            animator.SetFloat("MoveX", Mathf.Abs(lastMovement.x));
            animator.SetFloat("MoveY", lastMovement.y);
            animator.SetFloat("Speed", movement.magnitude);
        }

        // --- NUEVO: Lógica de Sonido de Pasos ---
        if (movement.magnitude > 0)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                if (SoundManager.instance != null)
                    SoundManager.instance.PlaySFX(SoundManager.instance.footstepClip);

                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // Si nos paramos, reseteamos el reloj
        }
    }

    void LateUpdate()
    {
        if (movement.magnitude > 0)
        {
            visualTransform.localScale = new Vector3(facingDir, 1f + Mathf.Sin(Time.time * 20f) * 0.1f, 1f);
        }
        else
        {
            visualTransform.localScale = new Vector3(facingDir, 1f, 1f);
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = moveSpeed;

        if (health != null && health.isShielded)
        {
            currentSpeed = moveSpeed * 0.5f;
        }

        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }
}