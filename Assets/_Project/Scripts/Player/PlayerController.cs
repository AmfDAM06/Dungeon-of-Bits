using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración")]
    public float moveSpeed = 5f;

    [Header("Animación por Código")]
    public Transform visualTransform;

    private Rigidbody2D rb;
    private Vector2 movement;
    private float facingDir = 1f;

    private Animator animator;
    private Vector2 lastMovement = new Vector2(0, -1);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (visualTransform == null) visualTransform = transform;

        animator = visualTransform.GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Recogemos el movimiento
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // 2. Decidimos la dirección del Flip
        if (movement.x < 0) facingDir = -1f;
        else if (movement.x > 0) facingDir = 1f;

        // 3. Memoria direccional
        if (movement.magnitude > 0)
        {
            lastMovement = movement;
        }

        // 4. Mandamos las órdenes al Animator
        if (animator != null)
        {
            animator.SetFloat("MoveX", Mathf.Abs(lastMovement.x));
            animator.SetFloat("MoveY", lastMovement.y);
            animator.SetFloat("Speed", movement.magnitude);
        }
    }

    // NUEVO: Esto se ejecuta DESPUÉS del Animator. ¡Aquí nadie nos quita la escala!
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
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}