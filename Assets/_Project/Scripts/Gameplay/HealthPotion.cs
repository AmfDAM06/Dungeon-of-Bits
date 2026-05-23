using System.Collections;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [Header("Configuración del Objeto")]
    [Tooltip("Escribe: Health, Invis, Strength o Bomb")]
    public string itemType = "Health";

    public float pickupDelay = 0.8f;
    private bool canBePickedUp = false;

    private void Start()
    {
        StartCoroutine(EnablePickupRoutine());
    }

    private IEnumerator EnablePickupRoutine()
    {
        yield return new WaitForSeconds(pickupDelay);
        canBePickedUp = true;
    }

    private void TryPickup(GameObject playerObj)
    {
        if (!canBePickedUp) return;

        PlayerInventory inventory = playerObj.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(itemType, 1);

            // --- NUEVO: Sonido de recoger ---
            if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.pickupItemClip);

            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) { if (collision.CompareTag("Player")) TryPickup(collision.gameObject); }
    private void OnTriggerEnter2D(Collider2D collision) { if (collision.CompareTag("Player")) TryPickup(collision.gameObject); }
    private void OnCollisionEnter2D(Collision2D collision) { if (collision.gameObject.CompareTag("Player")) TryPickup(collision.gameObject); }
}