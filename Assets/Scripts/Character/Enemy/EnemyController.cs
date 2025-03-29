using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("stats")] 
    [SerializeField] private float curentSpeed = 0;
    public Vector2 movementInput { get; set; }

    [Header("Attack")] 
    [SerializeField] private bool isAttack = true;
    [SerializeField] private float attackCoolDuration = 1;

    [Header("Knockback")] 
    [SerializeField] private bool isKnockback = true;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.1f;

    

    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private SpriteRenderer sr;
    private Animator anim;
    public bool isDead { get; set; }
    private bool isHurt;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!isDead && !isHurt)
        {
            Move();
        }
        
        SetAnimation();
    }

    void Move()
    {
        if (movementInput.magnitude > 0.1f && curentSpeed >= 0)
        {
            rb.velocity = movementInput * curentSpeed;
            //flip character
            if (movementInput.x < 0)
            {
                sr.flipX = true;
            }

            if (movementInput.x > 0)
            {
                sr.flipX = false;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Attack()
    {
        if (isAttack)
        {
            isAttack = false;
            StartCoroutine(nameof(AttackCoroutine));
        }
    }
    
    void SetAnimation()
    {
        anim.SetBool("isWalk", movementInput.magnitude > 0);
        anim.SetBool("isDead", isDead);
    }

    IEnumerator AttackCoroutine()
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackCoolDuration);
        isAttack = true;
    }

    public void EnemyHurt()
    {
        isHurt = true;
        anim.SetTrigger("Hurt");
    }

    public void Knockback(Vector3 pos)
    {
        //knockback
        if (!isKnockback || isDead)
        {
            return;
        }

        StartCoroutine(KnockbackCoroutine(pos));
    }

    IEnumerator KnockbackCoroutine(Vector3 pos)
    {
        var direction = (transform.position - pos).normalized;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isHurt = false;
    }

    public void EnemyDead()
    {
        rb.velocity = Vector2.zero;
        isDead = true;
        enemyCollider.enabled = false; //disable the collider
        // Make sure this runs only once on death
        FindObjectOfType<HUDController>().AddScore(10);
        FindObjectOfType<EnemySpawner>()?.EnemyDied();


    }

    public void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }
}
