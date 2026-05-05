using UnityEngine;
using System.Collections;

public class RobotAutomaton : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // --- CÓDIGO TEMPORAL PARA PROBAR EL FLIP ---
        // Puedes probar a pulsar las flechas del teclado para ver cómo gira.
        // Borraremos este Update cuando programemos el menú del puzle.
        if (Input.GetKeyDown(KeyCode.LeftArrow)) FaceDirection(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) FaceDirection(Vector2.right);
    }

    // --- FUNCIÓN PARA GIRAR AL ROBOT ---
    public void FaceDirection(Vector2 direction)
    {
        if (spriteRenderer == null) return;

        // Si la orden es ir a la izquierda (X negativa), activamos el modo espejo (Flip)
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        // Si la orden es ir a la derecha (X positiva), quitamos el modo espejo
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        // Nota: Si el robot se mueve arriba/abajo (eje Y), dejamos que siga mirando 
        // hacia la última dirección horizontal que tuvo.
    }
}