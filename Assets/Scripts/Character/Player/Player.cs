using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("MeleeAttack")] 
    public float meleeAttackDamage; // damage melee
    public Vector2 attackSize = new Vector2(1f, 1f); //attack size

    public float offsetX = 1f; //offsetX
    public float offsetY = 1f; //offsetY
    public LayerMask enemyLayer; // enemy layer

    private Vector2 AttackAreaPos;
    
    void MeleeAttackAnimEvent(float isAttack)
    {
        //offset center
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

    //Drawing for test
    private void OnDrawGizmosSelected()
    {
        
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(AttackAreaPos, attackSize);
    }
}
