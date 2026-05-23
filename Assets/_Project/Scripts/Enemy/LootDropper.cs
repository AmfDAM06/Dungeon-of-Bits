using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [Header("Botín Posible")]
    // Cambiamos el GameObject único por un Array para meter varias pociones
    public GameObject[] lootPrefabs;

    [Range(0f, 100f)]
    public float dropChance = 50f;

    public void TryDropLoot()
    {
        // Comprobamos que la lista tenga al menos un objeto
        if (lootPrefabs != null && lootPrefabs.Length > 0)
        {
            float randomValue = Random.Range(0f, 100f);
            if (randomValue <= dropChance)
            {
                // Elegimos un número al azar entre 0 y el total de objetos en la lista
                int randomIndex = Random.Range(0, lootPrefabs.Length);

                // Soltamos el objeto ganador
                Instantiate(lootPrefabs[randomIndex], transform.position, Quaternion.identity);
            }
        }
    }
}