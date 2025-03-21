using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField]protected float maxHealth;
    [SerializeField]protected float currentHealth;
    
    [Header("Invulnerability")]
    public bool invulnerable;
    
    public float invulnerableDuration;
    
    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (invulnerable)
        {
            return;
        }
        
        currentHealth -= damage;
        StartCoroutine(nameof(InvulnerableCoroutine));
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        currentHealth = 0f;
        Destroy(this.gameObject);
    }

    protected virtual IEnumerator InvulnerableCoroutine()
    {
        invulnerable = true;
        //wait for invulnerable time
        yield return new WaitForSeconds(invulnerableDuration);

        invulnerable = false;
    }
}
