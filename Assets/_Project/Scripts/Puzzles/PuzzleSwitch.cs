using System.Collections;
using UnityEngine;

public class PuzzleSwitch : MonoBehaviour
{
    public enum SwitchType { Pressure, Melee }

    [Header("Configuración")]
    public SwitchType type = SwitchType.Pressure;
    public string requiredTag = "Pushable";

    [Header("Animación")]
    public Sprite[] frames;
    public float frameRate = 0.05f;

    public bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private bool isAnimating = false;

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
        if (type == SwitchType.Melee && !isAnimating) SetState(!isActivated);
    }

    public void SetState(bool active)
    {
        if (isActivated == active) return;

        isActivated = active;

        // --- NUEVO: Sonido de interruptor ---
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