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

    // Guardamos hacia dónde mira (1 = derecha, -1 = izquierda)
    private float facingDir = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (visualTransform == null) visualTransform = transform;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // --- SISTEMA DE FLIP FÍSICO (ESCALA) ---
        // Si vamos a la izquierda, la escala X será negativa (-1)
        if (movement.x < 0) facingDir = -1f;
        // Si vamos a la derecha, la escala X será positiva (1)
        else if (movement.x > 0) facingDir = 1f;

        // Efecto de tambaleo al caminar + Flip integrado
        if (movement.magnitude > 0)
        {
            // Aplicamos la dirección (facingDir) a la X, y el tambaleo a la Y
            visualTransform.localScale = new Vector3(facingDir, 1f + Mathf.Sin(Time.time * 20f) * 0.1f, 1f);
        }
        else
        {
            // Si está quieto, mantiene hacia dónde miraba (facingDir) pero sin tambaleo
            visualTransform.localScale = new Vector3(facingDir, 1f, 1f);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}