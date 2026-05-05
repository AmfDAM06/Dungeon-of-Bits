using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Configuración del Escudo")]
    public KeyCode shieldKey = KeyCode.LeftShift; // Tecla para cubrirse
    public GameObject shieldVisual; // Arrastra aquí un sprite de un escudo azul/burbuja

    private Health playerHealth;

    void Start()
    {
        playerHealth = GetComponent<Health>();
        if (shieldVisual != null) shieldVisual.SetActive(false); // Apagado por defecto
    }

    void Update()
    {
        // Mientras mantienes pulsada la tecla
        if (Input.GetKeyDown(shieldKey))
        {
            playerHealth.isShielded = true;
            if (shieldVisual != null) shieldVisual.SetActive(true);
        }
        // Cuando sueltas la tecla
        else if (Input.GetKeyUp(shieldKey))
        {
            playerHealth.isShielded = false;
            if (shieldVisual != null) shieldVisual.SetActive(false);
        }
    }
}