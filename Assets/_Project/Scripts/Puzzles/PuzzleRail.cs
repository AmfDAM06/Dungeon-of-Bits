using UnityEngine;
using System.Collections;

public class PuzzleRail : MonoBehaviour
{
    public enum RailType { Straight, Curve }

    [Header("Configuración")]
    public RailType railType = RailType.Straight;
    public float rotationSpeed = 0.15f;

    [Header("Solo para Raíles Rectos")]
    [Tooltip("Activa esto si el dibujo de tu raíl (sin rotar) va de Izquierda a Derecha. Desactívalo si va de Arriba a Abajo.")]
    public bool isHorizontalByDefault = true;

    private bool isRotating = false;

    public void RotateRail()
    {
        if (isRotating) return;

        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.rotateRailClip);

        StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        isRotating = true;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, -90f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rotationSpeed;
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        transform.rotation = endRot;
        isRotating = false;
    }
}