using System.Collections;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [Header("Potion Settings")]
    public int healingAmount = 1;
    public float pickupDelay = 0.8f; // Tiempo en segundos antes de poder cogerla

    private bool canBePickedUp = false;

    private void Start()
    {
        // Al nacer la poción, empezamos la cuenta atrás
        StartCoroutine(EnablePickupRoutine());
    }

    private IEnumerator EnablePickupRoutine()
    {
        yield return new WaitForSeconds(pickupDelay);
        canBePickedUp = true;
    }

    // Usamos Stay2D para que te la bebas incluso si te quedas quieto encima esperando
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Si aún no se puede recoger, abortamos
        if (!canBePickedUp) return;

        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null)
            {
                bool wasHealed = playerHealth.Heal(healingAmount);

                if (wasHealed)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}