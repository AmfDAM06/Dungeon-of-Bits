using System.Collections;
using UnityEngine;

public class PuzzleSwitch : MonoBehaviour
{
    public enum SwitchType { Pressure, Melee }

    [Header("Configuración")]
    public SwitchType type = SwitchType.Pressure;
    public string requiredTag = "Pushable";

    // --- NUEVO: ¿Va pegado en la pared? ---
    public bool isWallMounted = false;

    [Header("Animación")]
    public Sprite[] frames;
    public float frameRate = 0.05f;

    public bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private bool isAnimating = false;

    // --- NUEVO: Temporizador para evitar pulsaciones múltiples ---
    private float lastToggleTime = -999f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (frames.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = frames[0];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == SwitchType.Pressure && !isActivated && collision.CompareTag(requiredTag)) SetState(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (type == SwitchType.Pressure && isActivated && collision.CompareTag(requiredTag)) SetState(false);
    }

    public void ToggleByHit()
    {
        // --- NUEVO: Solo se activa si ha pasado medio segundo desde el último golpe ---
        if (type == SwitchType.Melee && !isAnimating && Time.time >= lastToggleTime + 0.5f)
        {
            lastToggleTime = Time.time;
            SetState(!isActivated);
        }
    }

    public void SetState(bool active)
    {
        if (isActivated == active) return;

        isActivated = active;

        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.switchToggleClip);

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(AnimateSwitch(active));
        }
    }

    private IEnumerator AnimateSwitch(bool forward)
    {
        isAnimating = true;
        if (frames.Length > 0 && spriteRenderer != null)
        {
            if (forward)
            {
                for (int i = 0; i < frames.Length; i++) { spriteRenderer.sprite = frames[i]; yield return new WaitForSeconds(frameRate); }
            }
            else
            {
                for (int i = frames.Length - 1; i >= 0; i--) { spriteRenderer.sprite = frames[i]; yield return new WaitForSeconds(frameRate); }
            }
        }
        isAnimating = false;
    }

    public void ResetSwitch()
    {
        StopAllCoroutines();
        isAnimating = false;
        isActivated = false;
        if (frames.Length > 0 && spriteRenderer != null) spriteRenderer.sprite = frames[0];
    }
}