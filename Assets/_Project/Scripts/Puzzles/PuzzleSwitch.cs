using System.Collections;
using UnityEngine;

public class PuzzleSwitch : MonoBehaviour
{
    public enum SwitchType { Pressure, Melee } // Desplegable para elegir el tipo en el Inspector

    [Header("Configuración")]
    public SwitchType type = SwitchType.Pressure;
    public string requiredTag = "Pushable"; // Solo se usa si es tipo Pressure

    [Header("Animación")]
    public Sprite[] frames; // ¡Aquí arrastraremos los 5 fotogramas de tu palanca!
    public float frameRate = 0.05f; // Velocidad de la animación

    public bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private bool isAnimating = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Si hay fotogramas asignados, ponemos el primero
        if (frames.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = frames[0];
        }
    }

    // --- LÓGICA DE PRESIÓN (Cajas o Jugador) ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == SwitchType.Pressure && !isActivated && collision.CompareTag(requiredTag))
        {
            SetState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (type == SwitchType.Pressure && isActivated && collision.CompareTag(requiredTag))
        {
            SetState(false);
        }
    }

    // --- LÓGICA DE GOLPE (Espada) ---
    public void ToggleByHit()
    {
        // Solo reacciona si es una palanca y no se está moviendo ya
        if (type == SwitchType.Melee && !isAnimating)
        {
            SetState(!isActivated); // Cambia al estado contrario
        }
    }

    // --- SISTEMA DE ANIMACIÓN POR CÓDIGO ---
    private void SetState(bool active)
    {
        isActivated = active;
        Debug.Log(gameObject.name + " activado: " + isActivated);

        // Iniciamos la animación
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
            if (forward) // Animación hacia adelante (bajando palanca)
            {
                for (int i = 0; i < frames.Length; i++)
                {
                    spriteRenderer.sprite = frames[i];
                    yield return new WaitForSeconds(frameRate);
                }
            }
            else // Animación hacia atrás (subiendo palanca)
            {
                for (int i = frames.Length - 1; i >= 0; i--)
                {
                    spriteRenderer.sprite = frames[i];
                    yield return new WaitForSeconds(frameRate);
                }
            }
        }
        isAnimating = false;
    }
    // --- MÉTODO PARA RESETEAR ---
    public void ResetSwitch()
    {
        StopAllCoroutines(); // Detenemos la animación por si estaba a medias
        isAnimating = false;
        isActivated = false;

        // Volvemos a poner el primer sprite (apagado/sin pisar)
        if (frames.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = frames[0];
        }
    }
}