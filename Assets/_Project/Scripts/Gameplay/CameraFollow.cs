using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // --- NUEVO: Instancia pública para poder llamarla desde cualquier sitio ---
    public static CameraFollow instance;

    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Juice (Screen Shake)")]
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private Vector3 shakeOffset;

    void Awake()
    {
        // Configuramos la instancia para que sea accesible de forma global
        if (instance == null) instance = this;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // --- NUEVO: Lógica de Temblor de Cámara ---
            if (shakeDuration > 0)
            {
                // Generamos una vibración aleatoria
                shakeOffset = Random.insideUnitSphere * shakeMagnitude;
                shakeOffset.z = 0f; // Importante en 2D: evitar que la cámara tiemble hacia adelante/atrás

                // Le sumamos el temblor a la posición de seguimiento
                transform.position = smoothedPosition + shakeOffset;

                // Restamos el tiempo que lleva temblando usando "unscaledDeltaTime" para que el Hit-Stop no pause el temblor
                shakeDuration -= Time.unscaledDeltaTime;
            }
            else
            {
                // Si no hay temblor, se mueve de forma normal
                shakeDuration = 0f;
                transform.position = smoothedPosition;
            }
        }
    }

    // --- NUEVA FUNCIÓN: Cualquier script puede llamar a esta función para sacudir la pantalla ---
    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}