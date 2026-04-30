using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración")]
    public float moveSpeed = 5f; // Velocidad del jugador

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Capturamos el input de las teclas (W, A, S, D o Flechas)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalizamos para que no vaya más rápido al moverse en diagonal
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Movemos al personaje usando las físicas
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}