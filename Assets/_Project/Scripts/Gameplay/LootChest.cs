using UnityEngine;
using System.Collections.Generic;

public class LootChest : MonoBehaviour
{
    [Header("Botín del Cofre")]
    public GameObject[] lootPrefabs;

    [Header("Configuración Visual")]
    public Sprite openSprite;
    public Sprite closedSprite;
    public string requiredTag = "Player";

    public int lootAmount = 1;

    private bool isOpen = false;
    private bool isPlayerNear = false; // <-- NUEVO: Para saber si estás pegado al cofre
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && closedSprite != null) spriteRenderer.sprite = closedSprite;

        animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // --- NUEVO: Solo se abre si estás cerca y pulsas la E ---
        if (isPlayerNear && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    // Detectamos cuando el jugador se acerca
    private void OnTriggerEnter2D(Collider2D collision) { if (collision.CompareTag(requiredTag)) isPlayerNear = true; }
    private void OnCollisionEnter2D(Collision2D collision) { if (collision.gameObject.CompareTag(requiredTag)) isPlayerNear = true; }

    // Detectamos cuando el jugador se aleja
    private void OnTriggerExit2D(Collider2D collision) { if (collision.CompareTag(requiredTag)) isPlayerNear = false; }
    private void OnCollisionExit2D(Collision2D collision) { if (collision.gameObject.CompareTag(requiredTag)) isPlayerNear = false; }

    public void OpenChest()
    {
        if (isOpen) return;
        isOpen = true;

        if (SoundManager.instance != null && SoundManager.instance.hitClip != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.hitClip);
        }

        if (animator != null) animator.SetTrigger("Open");
        else if (spriteRenderer != null && openSprite != null) spriteRenderer.sprite = openSprite;

        SpawnLoot();
    }

    private void SpawnLoot()
    {
        if (lootPrefabs != null && lootPrefabs.Length > 0)
        {
            for (int i = 0; i < lootAmount; i++)
            {
                int randomIndex = Random.Range(0, lootPrefabs.Length);
                GameObject randomLoot = lootPrefabs[randomIndex];

                // Hacemos que caiga justo enfrente del cofre (en la Y negativa)
                Vector3 spawnPosition = transform.position + new Vector3(0f, -0.8f, 0f);

                Instantiate(randomLoot, spawnPosition, Quaternion.identity);
            }
        }
    }
}