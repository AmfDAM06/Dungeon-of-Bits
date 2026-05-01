using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f); // -10 en Z es crucial en 2D para que la cámara no atraviese el suelo

    void LateUpdate() // Usamos LateUpdate para la cámara para que se mueva DESPUÉS de que el jugador se haya movido
    {
        // 1. Si no tiene objetivo (porque el jugador acaba de spawnear), lo busca usando el Tag
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        // 2. Si ya encontró al jugador, lo sigue suavemente
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            // Lerp hace que el movimiento sea suave y no brusco
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }
}