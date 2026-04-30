using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración")]
    public float moveSpeed = 5f;

    [Header("Animación por Código")]
    public Transform visualTransform; // Arrastra aquí al Player en el Inspector

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Si no asignas nada en el inspector, coge el transform de este objeto
        if (visualTransform == null) visualTransform = transform;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        // Efecto de tambaleo al caminar
        if (movement.magnitude > 0)
        {
            // Escala dinámicamente el eje Y usando la función Seno para dar pequeños saltitos
            visualTransform.localScale = new Vector3(1f, 1f + Mathf.Sin(Time.time * 20f) * 0.1f, 1f);
        }
        else
        {
            // Vuelve a la normalidad si está quieto
            visualTransform.localScale = Vector3.one;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}