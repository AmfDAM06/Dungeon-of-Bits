using UnityEngine;

public class LogicDoor : MonoBehaviour
{
    [Header("Configuración")]
    public PuzzleSwitch[] requiredSwitches;

    private bool isOpen = false;
    private BoxCollider2D doorCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (requiredSwitches == null || requiredSwitches.Length == 0) return;

        int activatedCount = 0;

        // Contamos cuántos interruptores del puzle están encendidos ahora mismo
        foreach (PuzzleSwitch sw in requiredSwitches)
        {
            if (sw != null && sw.isActivated)
            {
                activatedCount++;
            }
        }

        // --- NUEVO: Enviamos el recuento en tiempo real a la interfaz ---
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdatePuzzleUI(activatedCount, requiredSwitches.Length);
        }

        // Si todos están activados, abrimos la puerta
        if (activatedCount == requiredSwitches.Length)
        {
            if (!isOpen) OpenDoor();
        }
        else
        {
            if (isOpen) CloseDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        if (doorCollider != null) doorCollider.enabled = false;
        if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);
        Debug.Log("La puerta lógica se ha ABIERTO.");
    }

    private void CloseDoor()
    {
        isOpen = false;
        if (doorCollider != null) doorCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        Debug.Log("La puerta lógica se ha CERRADO.");
    }

    public void ResetDoor()
    {
        CloseDoor();
    }
}