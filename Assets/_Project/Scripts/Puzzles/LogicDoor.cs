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
        if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f);

        // --- NUEVO ---
        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.logicDoorClip);
    }

    private void CloseDoor()
    {
        isOpen = false;
        if (doorCollider != null) doorCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        // --- NUEVO ---
        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.logicDoorClip);
    }

    public void ResetDoor() { CloseDoor(); }
}