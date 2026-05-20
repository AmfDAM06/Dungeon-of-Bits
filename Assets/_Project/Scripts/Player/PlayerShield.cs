using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Configuración del Escudo")]
    public KeyCode shieldKey = KeyCode.LeftShift; // Tecla para cubrirse
    public GameObject shieldVisual;
    public Animator animator; // NUEVO: Para saber hacia dónde miramos

    [Header("Posiciones del Escudo (NUEVO)")]
    // Ajusta esto desde el Inspector igual que con la espada
    public Vector3 sideShieldOffset = new Vector3(1f, 0f, 0f);
    public Vector3 upShieldOffset = new Vector3(0f, 1f, 0f);
    public Vector3 downShieldOffset = new Vector3(0f, -1f, 0f);

    public Vector3 sideShieldRotation = new Vector3(0f, 0f, 0f);
    public Vector3 upShieldRotation = new Vector3(0f, 0f, 90f);
    public Vector3 downShieldRotation = new Vector3(0f, 0f, -90f);

    private Health playerHealth;

    void Start()
    {
        playerHealth = GetComponent<Health>();
        if (shieldVisual != null) shieldVisual.SetActive(false); // Apagado por defecto

        // Buscamos el Animator por si no lo has arrastrado manualmente
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Activar / Desactivar el escudo
        if (Input.GetKeyDown(shieldKey))
        {
            playerHealth.isShielded = true;
            if (shieldVisual != null) shieldVisual.SetActive(true);
        }
        else if (Input.GetKeyUp(shieldKey))
        {
            playerHealth.isShielded = false;
            if (shieldVisual != null) shieldVisual.SetActive(false);
        }

        // 2. NUEVO: Si el escudo está activo, lo movemos hacia donde mire el personaje
        if (playerHealth.isShielded && shieldVisual != null && animator != null)
        {
            float moveY = animator.GetFloat("MoveY");

            // Si miramos hacia Arriba
            if (moveY > 0.1f)
            {
                shieldVisual.transform.localPosition = upShieldOffset;
                shieldVisual.transform.localRotation = Quaternion.Euler(upShieldRotation);
            }
            // Si miramos hacia Abajo
            else if (moveY < -0.1f)
            {
                shieldVisual.transform.localPosition = downShieldOffset;
                shieldVisual.transform.localRotation = Quaternion.Euler(downShieldRotation);
            }
            // Si miramos a los Lados
            else
            {
                shieldVisual.transform.localPosition = sideShieldOffset;
                shieldVisual.transform.localRotation = Quaternion.Euler(sideShieldRotation);
            }
        }
    }
}