using UnityEngine;

public class LogicDoor : MonoBehaviour
{
    [Header("Condiciones Lógicas (AND)")]
    [Tooltip("Arrastra aquí los interruptores que deben estar pisados a la vez")]
    public PuzzleSwitch[] requiredSwitches; // ARRAY: Una lista de interruptores

    [Header("Visuales de la Puerta")]
    public Sprite closedSprite; // Sprite de puerta cerrada
    public Sprite openedSprite; // Sprite de puerta abierta (o suelo normal)

    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;
    private bool isOpen = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();

        // Empezamos con la puerta cerrada
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }
    }

    void Update()
    {
        // Si la puerta está cerrada, estamos evaluando constantemente la condición
        if (!isOpen)
        {
            // Llama a la función que actúa como nuestro operador AND
            if (CheckAllSwitches())
            {
                OpenDoor();
            }
        }
    }

    // MÉTODO LÓGICO: Comprueba el Array con un Bucle
    private bool CheckAllSwitches()
    {
        // Si no hemos asignado ningún interruptor, devolvemos falso por seguridad
        if (requiredSwitches.Length == 0) return false;

        // BUCLE FOREACH: "Por cada interruptor en la lista..."
        foreach (PuzzleSwitch sw in requiredSwitches)
        {
            // LÓGICA AND: Si encontramos tan solo UN interruptor sin pisar, la condición falla
            if (!sw.isActivated)
            {
                return false;
            }
        }

        // Si el bucle termina sin problemas, significa que TODOS están pisados
        return true;
    }

    private void OpenDoor()
    {
        isOpen = true;

        // Cambiamos el dibujo para que parezca abierta
        if (spriteRenderer != null && openedSprite != null)
        {
            spriteRenderer.sprite = openedSprite;
        }

        // Desactivamos las físicas para que el jugador pueda atravesarla
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        Debug.Log("¡Condición IF cumplida! Puerta Lógica abierta.");
    }
    // --- MÉTODO PARA RESETEAR ---
    public void ResetDoor()
    {
        isOpen = false;

        // Volvemos a poner el dibujo de la puerta cerrada
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }

        // Volvemos a activar la colisión para que bloquee el paso
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
    }
}