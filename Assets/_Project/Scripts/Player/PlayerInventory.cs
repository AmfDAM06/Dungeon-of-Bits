using System.Collections;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static int globalHealthPotions = 0;
    public static int globalInvisPotions = 0;
    public static int globalStrengthPotions = 0;
    public static int globalBombs = 0;

    [Header("Cantidades en el Inventario")]
    public int healthPotions = 0;
    public int invisPotions = 0;
    public int strengthPotions = 0;
    public int bombs = 0;

    [Header("Prefabs para usar")]
    public GameObject bombPrefabToSpawn;

    private Health playerHealth;

    void Start()
    {
        playerHealth = GetComponent<Health>();

        // --- LA SOLUCIÓN ---
        // Si estamos en el piso 1 y has escrito números en el Inspector, los respetamos para hacer trampas/pruebas.
        if (UIManager.currentFloor == 1 && (healthPotions > 0 || invisPotions > 0 || strengthPotions > 0 || bombs > 0))
        {
            SyncGlobals();
        }
        else
        {
            // Si no hay trampas, cargamos la memoria global normal
            healthPotions = globalHealthPotions;
            invisPotions = globalInvisPotions;
            strengthPotions = globalStrengthPotions;
            bombs = globalBombs;
        }

        UpdateUI();
    }

    void Update()
    {
        // 1. VIDA (Acepta el '1' de arriba y el '1' del teclado numérico)
        if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) && healthPotions > 0)
        {
            // Solo te deja beberla si te falta vida
            if (playerHealth != null && playerHealth.GetCurrentHealth() < playerHealth.maxHealth)
            {
                playerHealth.Heal(1);
                healthPotions--;
                SyncGlobals();
                UpdateUI();

                if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.drinkPotionClip);
            }
        }

        // 2. INVISIBILIDAD
        if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) && invisPotions > 0)
        {
            if (playerHealth != null && !playerHealth.isInvisible)
            {
                StartCoroutine(InvisibilityRoutine(10f));
                invisPotions--;
                SyncGlobals();
                UpdateUI();

                if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.drinkPotionClip);
            }
        }

        // 3. FUERZA
        if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) && strengthPotions > 0)
        {
            DamageDealer swordDamage = GetComponentInChildren<DamageDealer>(true);
            if (swordDamage != null && !swordDamage.isBuffed)
            {
                StartCoroutine(StrengthRoutine(swordDamage, 10f));
                strengthPotions--;
                SyncGlobals();
                UpdateUI();

                if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.drinkPotionClip);
            }
        }

        // 4. BOMBA
        if ((Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) && bombs > 0)
        {
            if (bombPrefabToSpawn != null)
            {
                Instantiate(bombPrefabToSpawn, transform.position, Quaternion.identity);
                bombs--;
                SyncGlobals();
                UpdateUI();
            }
            else
            {
                Debug.LogWarning("¡Intento usar una bomba pero la casilla 'Bomb Prefab To Spawn' está vacía en el Inspector!");
            }
        }
    }

    public void AddItem(string itemType, int amount)
    {
        switch (itemType)
        {
            case "Health": healthPotions += amount; break;
            case "Invis": invisPotions += amount; break;
            case "Strength": strengthPotions += amount; break;
            case "Bomb": bombs += amount; break;
        }
        SyncGlobals();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateInventoryUI(healthPotions, invisPotions, strengthPotions, bombs);
        }
    }

    private void SyncGlobals()
    {
        globalHealthPotions = healthPotions;
        globalInvisPotions = invisPotions;
        globalStrengthPotions = strengthPotions;
        globalBombs = bombs;
    }

    public static void ResetInventory()
    {
        globalHealthPotions = 0;
        globalInvisPotions = 0;
        globalStrengthPotions = 0;
        globalBombs = 0;
    }

    private IEnumerator InvisibilityRoutine(float duration)
    {
        playerHealth.isInvisible = true;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.4f);

        yield return new WaitForSeconds(duration);

        if (sr != null) sr.color = Color.white;
        playerHealth.isInvisible = false;
    }

    private IEnumerator StrengthRoutine(DamageDealer sword, float duration)
    {
        sword.isBuffed = true;
        sword.damageAmount *= 2;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = Color.cyan;

        yield return new WaitForSeconds(duration);

        if (sr != null) sr.color = Color.white;
        sword.damageAmount /= 2;
        sword.isBuffed = false;
    }
}