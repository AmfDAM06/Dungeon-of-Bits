using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject swordObject; // Aquí arrastraremos el objeto Sword

    [Header("Estadísticas de Combate")]
    public float attackDuration = 0.15f; // Cuánto tiempo está visible la espada
    public float attackCooldown = 0.4f;  // Tiempo de recarga entre ataques

    private bool isAttacking = false;

    void Update()
    {
        // Atacamos con el Clic Izquierdo del ratón o la barra Espaciadora
        if ((Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)) && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 1. Activamos la espada
        swordObject.SetActive(true);

        // 2. Esperamos lo que dura el "tajo"
        yield return new WaitForSeconds(attackDuration);

        // 3. Desactivamos la espada
        swordObject.SetActive(false);

        // 4. Esperamos el tiempo de recarga para no "spamear" el ataque
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }
}