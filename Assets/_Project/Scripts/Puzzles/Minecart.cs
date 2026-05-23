using UnityEngine;
using System.Collections;

public class Minecart : MonoBehaviour
{
    [Header("Configuración")]
    public float moveSpeed = 3f;
    public Vector2 startDirection = Vector2.up;
    public LayerMask railLayer;
    public LayerMask switchLayer;

    private Vector3 startPosition;
    private bool isActive = false;
    private Vector2 currentDirection;

    void Start() { startPosition = transform.position; }

    public void ActivateCart()
    {
        if (isActive) return;

        isActive = true;
        currentDirection = startDirection;

        if (SoundManager.instance != null) SoundManager.instance.PlayLoop(SoundManager.instance.minecartMoveClip);

        StartCoroutine(MoveCartRoutine());
    }

    private IEnumerator MoveCartRoutine()
    {
        while (isActive)
        {
            Vector3 targetPos = transform.position + (Vector3)currentDirection;

            float t = 0;
            Vector3 startPos = transform.position;
            while (t < 1f)
            {
                t += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }
            transform.position = targetPos;

            Collider2D switchCollider = Physics2D.OverlapCircle(transform.position, 0.1f, switchLayer);
            if (switchCollider != null)
            {
                PuzzleSwitch pSwitch = switchCollider.GetComponent<PuzzleSwitch>();
                if (pSwitch != null) pSwitch.SetState(true);

                isActive = false;
                if (SoundManager.instance != null) SoundManager.instance.StopLoop();
                yield break;
            }

            Collider2D railCollider = Physics2D.OverlapCircle(transform.position, 0.1f, railLayer);
            if (railCollider != null)
            {
                PuzzleRail rail = railCollider.GetComponent<PuzzleRail>();

                // --- NUEVO: Comprobamos si el raíl es seguro ---
                bool isSafe = CalculateNewDirection(rail);
                if (!isSafe)
                {
                    // Chocamos contra un raíl recto mal puesto
                    yield return new WaitForSeconds(0.2f);
                    ResetCart();
                }
            }
            else
            {
                // Nos salimos del camino (no hay raíles)
                yield return new WaitForSeconds(0.5f);
                ResetCart();
            }
        }
    }

    private bool CalculateNewDirection(PuzzleRail rail)
    {
        Vector2 localDir = rail.transform.InverseTransformDirection(currentDirection);

        if (rail.railType == PuzzleRail.RailType.Curve)
        {
            if (Mathf.Abs(localDir.y) > 0.5f) currentDirection = rail.transform.TransformDirection(new Vector2(-Mathf.Sign(localDir.y), 0));
            else currentDirection = rail.transform.TransformDirection(new Vector2(0, -Mathf.Sign(localDir.x)));

            currentDirection = new Vector2(Mathf.Round(currentDirection.x), Mathf.Round(currentDirection.y));
            return true;
        }
        else // RailType.Straight
        {
            // --- NUEVO: Lógica restrictiva para los raíles rectos ---
            if (rail.isHorizontalByDefault)
            {
                if (Mathf.Abs(localDir.x) > 0.5f) return true; // Avanza bien por el eje X local
                else return false; // Choca
            }
            else
            {
                if (Mathf.Abs(localDir.y) > 0.5f) return true; // Avanza bien por el eje Y local
                else return false; // Choca
            }
        }
    }

    private void ResetCart()
    {
        isActive = false;
        if (SoundManager.instance != null) SoundManager.instance.StopLoop();
        transform.position = startPosition;
    }
}