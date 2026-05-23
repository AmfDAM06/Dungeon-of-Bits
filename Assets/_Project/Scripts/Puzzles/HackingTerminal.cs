using UnityEngine;
using TMPro;

public class HackingTerminal : MonoBehaviour
{
    [Header("Terminal UI")]
    public GameObject hackingCanvas;
    public TMP_InputField inputField;
    public TextMeshProUGUI feedbackText;
    public static bool isTerminalOpen = false;

    [Header("Sci-Fi Door")]
    public GameObject sciFiDoor;
    public Sprite doorOpenSprite;

    private bool isPlayerNear = false;
    private bool isHacked = false;

    // Guardamos la referencia directa a tu script de movimiento
    private PlayerController playerControllerScript;

    void Start()
    {
        if (hackingCanvas != null)
        {
            hackingCanvas.SetActive(false);
        }

        // Ejecuta CheckCode cuando el jugador pulsa Enter
        inputField.onSubmit.AddListener(CheckCode);
    }

    void Update()
    {
        // Solo podemos interactuar si estamos cerca y la puerta no ha sido hackeada aún
        if (isPlayerNear && !isHacked)
        {
            // 1. ABRIR LA TERMINAL CON 'E' (Solo si está cerrada)
            if (!hackingCanvas.activeSelf && Input.GetKeyDown(KeyCode.E))
            {
                OpenTerminal();
            }
            // 2. CERRAR LA TERMINAL CON 'ESCAPE' (Solo si está abierta)
            else if (hackingCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTerminal();
            }
        }
    }

    private void OpenTerminal()
    {
        isTerminalOpen = true;
        hackingCanvas.SetActive(true);
        inputField.text = "";
        feedbackText.text = "> Awaiting input...";
        feedbackText.color = Color.white;

        // ¡Congelamos al jugador desactivando su script de movimiento!
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false;
        }

        // --- NUEVO: Sonido de iniciar la terminal ---
        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.terminalStartClip);

        // Seleccionamos automáticamente el InputField para escribir del tirón
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void CloseTerminal()
    {
        isTerminalOpen = false;
        hackingCanvas.SetActive(false);

        // ¡Descongelamos al jugador volviendo a activar su script!
        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true;
        }
    }

    public void CheckCode(string code)
    {
        // Limpiamos espacios en blanco accidentales
        code = code.Trim();

        if (code == "door.Open();")
        {
            feedbackText.color = Color.green;
            feedbackText.text = "> Access Granted. Executing Open() method.";

            // --- NUEVO: Sonido de éxito en el hackeo ---
            if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.terminalSuccessClip);

            OpenSciFiDoor();
            isHacked = true;

            // Cerramos la terminal tras 2 segundos para que el jugador vea el mensaje de éxito
            Invoke("CloseTerminal", 2f);
        }
        else
        {
            feedbackText.color = Color.red;
            feedbackText.text = "> Syntax error or undefined method.";

            // --- NUEVO: Sonido de error en el hackeo ---
            if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.terminalErrorClip);

            inputField.text = "";

            // Reactivamos el campo por si quiere volver a intentarlo sin usar el ratón
            inputField.Select();
            inputField.ActivateInputField();
        }
    }

    private void OpenSciFiDoor()
    {
        SpriteRenderer doorRenderer = sciFiDoor.GetComponent<SpriteRenderer>();
        if (doorRenderer != null) doorRenderer.sprite = doorOpenSprite;

        Collider2D doorCollider = sciFiDoor.GetComponent<Collider2D>();
        if (doorCollider != null) doorCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;

            // Cogemos la referencia a tu PlayerController al acercarnos
            playerControllerScript = collision.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            // Si el jugador se aleja (por ejemplo, si lo empuja un enemigo), la terminal se cierra
            CloseTerminal();
        }
    }
}