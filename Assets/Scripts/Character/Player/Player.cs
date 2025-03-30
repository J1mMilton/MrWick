using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("MeleeAttack")] 
    public float meleeAttackDamage; // base melee damage
    public Vector2 attackSize = new Vector2(1f, 1f); // attack area size

    public float offsetX = 1f; // attack offset X
    public float offsetY = 1f; // attack offset Y
    public LayerMask enemyLayer; // enemy layer

    private Vector2 AttackAreaPos;

    // New fields for temporary boosts.
    private float originalMeleeDamage;

    void Awake()
    {
        originalMeleeDamage = meleeAttackDamage;
    }

    void MeleeAttackAnimEvent(float isAttack)
    {
        // Calculate attack area center.
        AttackAreaPos = transform.position;
        AttackAreaPos.x += offsetX;
        AttackAreaPos.y += offsetY;
        
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(AttackAreaPos, attackSize, 0f, enemyLayer);

        foreach (Collider2D hitCollider in hitColliders)
        {
            hitCollider.GetComponent<Character>().TakeDamage(meleeAttackDamage * isAttack);
            hitCollider.GetComponent<EnemyController>().Knockback(transform.position);
        }
    }

    // Public method to recover health.
    public void RecoverHealth(int amount)
    {
        // Increase currentHealth but do not exceed maxHealth.
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        // Optionally update HUD here.
        Debug.Log("Recovered health: " + amount + " new health: " + currentHealth);
    }

    // Public method to add temporary invulnerability.
    public void AddInvulnerability(float duration)
    {
        StartCoroutine(InvulnerabilityPowerUp(duration));
    }

    private IEnumerator InvulnerabilityPowerUp(float duration)
    {
        // Set the invulnerable flag (from Character.cs).
        invulnerable = true;
        // Optionally change player appearance (e.g., flash)
        Debug.Log("Player is invulnerable for " + duration + " seconds.");
        yield return new WaitForSeconds(duration);
        invulnerable = false;
        Debug.Log("Invulnerability ended.");
    }

    // Public method to boost melee attack damage temporarily.
    public void AddAttackDamageBoost(float boostAmount, float duration)
    {
        StartCoroutine(AttackDamageBoostCoroutine(boostAmount, duration));
    }

    private IEnumerator AttackDamageBoostCoroutine(float boostAmount, float duration)
    {
        meleeAttackDamage += boostAmount;
        Debug.Log("Attack damage boosted to " + meleeAttackDamage);
        yield return new WaitForSeconds(duration);
        meleeAttackDamage = originalMeleeDamage;
        Debug.Log("Attack damage boost ended; damage reset to " + meleeAttackDamage);
    }

    private void Update()
    {
        // Update HUD health display.
        FindObjectOfType<HUDController>().UpdateHP((int)currentHealth);
    }

    // For debugging: show the attack area.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(AttackAreaPos, attackSize);
    }
}
