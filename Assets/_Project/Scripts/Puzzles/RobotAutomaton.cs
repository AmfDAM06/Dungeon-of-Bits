using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAutomaton : MonoBehaviour
{
    public enum Command { MoveForward, TurnLeft, TurnRight }

    [Header("Configuración")]
    public float moveSpeed = 3f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isExecuting = false;

    void Start()
    {
        // Guardamos su posición inicial para reiniciarlo si falla
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void ExecuteProgram(List<Command> program)
    {
        if (isExecuting) return;
        StartCoroutine(RunProgramRoutine(program));
    }

    private IEnumerator RunProgramRoutine(List<Command> program)
    {
        isExecuting = true;

        // Siempre empieza desde la posición inicial al ejecutar
        transform.position = startPosition;
        transform.rotation = startRotation;
        yield return new WaitForSeconds(0.5f);

        foreach (Command cmd in program)
        {
            if (cmd == Command.MoveForward)
            {
                // Vector3.up es "Hacia adelante" relativo a la rotación actual del robot
                Vector3 targetPos = transform.position + transform.up;

                // Animación de movimiento suave hacia la siguiente cuadrícula
                float t = 0;
                Vector3 currentPos = transform.position;
                while (t < 1f)
                {
                    t += Time.deltaTime * moveSpeed;
                    transform.position = Vector3.Lerp(currentPos, targetPos, t);
                    yield return null;
                }
                transform.position = targetPos; // Aseguramos que encaje perfecto
            }
            else if (cmd == Command.TurnLeft)
            {
                // Gira 90 grados a la izquierda en el eje Z (2D)
                transform.Rotate(0, 0, 90f);
            }
            else if (cmd == Command.TurnRight)
            {
                // Gira 90 grados a la derecha
                transform.Rotate(0, 0, -90f);
            }

            // Pausa entre línea de código y línea de código para que el jugador lo vea pensar
            yield return new WaitForSeconds(0.3f);
        }

        isExecuting = false;
    }
}