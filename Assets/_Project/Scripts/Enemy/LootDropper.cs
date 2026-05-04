using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [Header("Loot Settings")]
    public GameObject itemToDrop; // Arrastra el prefab de la poción aquí

    [Range(0f, 1f)]
    public float dropChance = 0.3f; // Probabilidad de soltar el objeto (0.3f = 30%)

    // Este método lo llamaremos justo antes de que el enemigo se destruya
    public void TryDropLoot()
    {
        // Si no hemos asignado ningún objeto, no hacemos nada
        if (itemToDrop == null) return;

        // Tiramos un dado "virtual" del 0 al 1
        float randomRoll = Random.value;

        // Si el número aleatorio es menor o igual a nuestra probabilidad, instanciamos el objeto
        if (randomRoll <= dropChance)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
            Debug.Log("Loot dropped at " + transform.position);
        }
    }
}