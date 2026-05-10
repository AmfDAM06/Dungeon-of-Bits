using System.Collections;
using UnityEngine;

public class LootChest : MonoBehaviour
{
    [Header("Animación del Cofre")]
    // Aquí pondremos los 3 sprites: [0] Cerrado, [1] Medio Abierto, [2] Abierto
    public Sprite[] chestFrames;
    public float animationSpeed = 0.15f;
    private SpriteRenderer spriteRenderer;

    [Header("Botín (Loot)")]
    // Ponemos un Array por si quieres que a veces suelte una poción y a veces otra cosa
    public GameObject[] possibleLoot;

    private bool isPlayerNear = false;
    private bool isOpened = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Empezamos con el cofre cerrado
        if (chestFrames.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = chestFrames[0];
        }
    }

    void Update()
    {
        // Si el jugador está cerca, no se ha abierto y pulsa 'E'
        if (isPlayerNear && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            // Bloqueamos el cofre para que no se abra dos veces
            isOpened = true;
            StartCoroutine(OpenChestRoutine());
        }
    }

    private IEnumerator OpenChestRoutine()
    {
        // 1. Reproducimos la animación frame a frame
        if (chestFrames.Length > 0 && spriteRenderer != null)
        {
            for (int i = 0; i < chestFrames.Length; i++)
            {
                spriteRenderer.sprite = chestFrames[i];
                yield return new WaitForSeconds(animationSpeed);
            }
        }

        // 2. Soltamos el botín una vez que el cofre está abierto del todo
        SpawnLoot();
    }

    private void SpawnLoot()
    {
        if (possibleLoot.Length > 0)
        {
            // Elegimos un objeto al azar de la lista de posibles botines
            int randomIndex = Random.Range(0, possibleLoot.Length);
            GameObject lootToSpawn = possibleLoot[randomIndex];

            if (lootToSpawn != null)
            {
                // Lo instanciamos ligeramente más abajo del cofre para que parezca que cae delante
                Vector3 spawnPosition = transform.position + new Vector3(0f, -0.5f, 0f);
                Instantiate(lootToSpawn, spawnPosition, Quaternion.identity);
            }
        }
    }

    // Detectar si el jugador está delante del cofre
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerNear = false;
    }
}