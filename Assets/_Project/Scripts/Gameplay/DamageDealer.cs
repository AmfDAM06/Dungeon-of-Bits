using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 1;
    public string targetTag = "Enemy";

    [Header("Configuración de Ataque")]
    public float attackCooldown = 1.5f; // Segundos entre ataque y ataque
    private float lastAttackTime = -999f; // Memoria de cuándo fue el último ataque

    // Detectan el primer impacto
    private void OnTriggerEnter2D(Collider2D collision) { TryDealDamage(collision.gameObject); }
    private void OnCollisionEnter2D(Collision2D collision) { TryDealDamage(collision.gameObject); }

    // NUEVO: Detectan si los objetos se quedan pegados rozándose
    private void OnTriggerStay2D(Collider2D collision) { TryDealDamage(collision.gameObject); }
    private void OnCollisionStay2D(Collision2D collision) { TryDealDamage(collision.gameObject); }

    private void TryDealDamage(GameObject targetObj)
    {
        // Las palancas y puzles no tienen cooldown, reaccionan al instante
        PuzzleSwitch puzzleSwitch = targetObj.GetComponent<PuzzleSwitch>();
        if (puzzleSwitch != null)
        {
            puzzleSwitch.ToggleByHit();
            return;
        }

        // Sistema de daño a entidades (Enemigos o Jugador)
        if (targetObj.CompareTag(targetTag))
        {
            // Comprobamos si ya ha pasado el tiempo suficiente desde el último golpe
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Health targetHealth = targetObj.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damageAmount);
                    lastAttackTime = Time.time; // Reiniciamos el cronómetro
                }
            }
        }
    }
}