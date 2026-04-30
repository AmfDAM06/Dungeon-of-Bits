using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform target; // Aquí arrastraremos a tu Player

    [Header("Configuración de Cámara")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f; // Contra más bajo, más "chicle" o suave será el seguimiento
    public Vector3 offset = new Vector3(0f, 0f, -10f); // La Z debe ser -10 para que la cámara no se meta dentro del mapa 2D

    // Usamos LateUpdate en lugar de Update para la cámara. 
    // Esto asegura que el jugador se mueva primero en Update/FixedUpdate, y la cámara reaccione DESPUÉS.
    void LateUpdate()
    {
        if (target != null)
        {
            // Calculamos la posición a la que debe ir la cámara
            Vector3 desiredPosition = target.position + offset;

            // Lerp hace una transición suave entre la posición actual y la deseada
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Aplicamos la nueva posición
            transform.position = smoothedPosition;
        }
    }
}
