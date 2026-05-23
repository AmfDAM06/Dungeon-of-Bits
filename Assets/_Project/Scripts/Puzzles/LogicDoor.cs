using UnityEngine;

public class LogicDoor : MonoBehaviour
{
    [Header("Configuración del Puzle")]
    public PuzzleSwitch[] requiredSwitches;

    [Header("Configuración Visual")]
    public Sprite closedSprite; // <-- NUEVO: Tu dibujo de la puerta cerrada
    public Sprite openSprite;   // <-- NUEVO: Tu dibujo de la puerta abierta

    private bool isOpen = false;
    private BoxCollider2D doorCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Nos aseguramos de que al empezar, muestre el sprite cerrado
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }
    }

    void Update()
    {
        if (requiredSwitches == null || requiredSwitches.Length == 0) return;

        int activatedCount = 0;

        foreach (PuzzleSwitch sw in requiredSwitches)
        {
            if (sw != null && sw.isActivated) activatedCount++;
        }

        if (UIManager.Instance != null) UIManager.Instance.UpdatePuzzleUI(activatedCount, requiredSwitches.Length);

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

        // --- CAMBIO: Ponemos el Sprite de la puerta abierta ---
        if (spriteRenderer != null && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
            spriteRenderer.color = Color.white; // Restauramos el color normal por si acaso
        }

        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.logicDoorClip);
    }

    private void CloseDoor()
    {
        isOpen = false;
        if (doorCollider != null) doorCollider.enabled = true;

        // --- CAMBIO: Ponemos el Sprite de la puerta cerrada ---
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }

        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.logicDoorClip);
    }

    public void ResetDoor() { CloseDoor(); }
}