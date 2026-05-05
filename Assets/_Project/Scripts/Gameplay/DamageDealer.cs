using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public int damageAmount = 1; // Cuánta vida quita este ataque
    public string targetTag = "Enemy"; // A quién queremos hacerle daño

    // 1. Detecta ataques "Fantasma" (Espadas, proyectiles mágicos...)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DealDamage(collision.gameObject);

        PuzzleSwitch puzzleSwitch = collision.GetComponent<PuzzleSwitch>();
        if (puzzleSwitch != null)
        {
            puzzleSwitch.ToggleByHit();
        }
    }

    // 2. Detecta choques "Sólidos" (El cuerpo del enemigo chocando contra ti)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DealDamage(collision.gameObject);
    }

    // Método centralizado para no repetir código
    private void DealDamage(GameObject target)
    {
        if (target.CompareTag(targetTag))
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damageAmount);
            }
        }
    }
}