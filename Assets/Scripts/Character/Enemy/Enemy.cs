using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

public class Enemy : Character
{
    public UnityEvent<Vector2> OnMovementInput;

    public UnityEvent OnAttack;
    
    [SerializeField]private Transform player;
    [SerializeField] private float chaseDistance = 3f;
    [SerializeField] private float attackDistance = 0.8f;

    private Seeker seeker;
    private List<Vector3> pathPointList;
    private int currentIndex = 0;
    private float pathGenerateInterval = 0.5f;
    private float pathGenerateTimer = 0f;

    [Header("Attack")] public float meleeAttackDamage;
    public LayerMask playerLayer;
    public float AttackCooldownDuration = 2f;

    private bool isAttack = true;
    private SpriteRenderer sr;

    private float attackRange = 20f;
    public Transform firePoint; // Where to shoot from
    public GameObject projectilePrefab;
    public float fireCooldown = 2f;
    private float nextFireTime = 0f;

    public Transform barrel;
    
    
    private Vector3 initialBarrelLocalPosition;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        sr = GetComponent<SpriteRenderer>();
        initialBarrelLocalPosition = barrel.localPosition; // store original position
    }

    private void Update()
    {
        if (player == null)
            return;
        float distance = Vector2.Distance(player.position, transform.position);

        // Determine if enemy should face right or left
        bool facingRight = player.position.x > transform.position.x;
        sr.flipX = !facingRight; // flips the sprite visuals only

        if (facingRight)
        {
            // Ensure X is positive for right side; Y remains unchanged.
            barrel.localPosition = new Vector3(Mathf.Abs(initialBarrelLocalPosition.x), initialBarrelLocalPosition.y, initialBarrelLocalPosition.z);
        }
        else
        {
            // Mirror the X coordinate; keep Y the same.
            barrel.localPosition = new Vector3(-Mathf.Abs(initialBarrelLocalPosition.x), initialBarrelLocalPosition.y, initialBarrelLocalPosition.z);
        }
        
        if (distance < chaseDistance)
        {
            AutoPath();
            if (pathPointList == null)
            {
                return;
            }
            
            if (distance <= attackDistance)
            {
                OnMovementInput?.Invoke(Vector2.zero);//stop moving when attacking
                if (isAttack)
                {
                    isAttack = false;
                    OnAttack?.Invoke();
                    // StartCoroutine(nameof(AttackCooldownCoroutine));
                }
                //flip direction when attacking
                float x = player.position.x - transform.position.x;
                if (x > 0)
                {
                    sr.flipX = false;
                }
                else
                {
                    sr.flipX = true;
                }
            }
            else
            {
                // chase player
                Vector2 direction = (pathPointList[currentIndex] - transform.position).normalized;
                OnMovementInput?.Invoke(direction); //transfer moving direction to EnemyController
            }
        }
        else
        {
            //Give up chasing
            OnMovementInput?.Invoke(Vector2.zero);
        }

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            AttackPlayer();
        }
        {
            
        }
    }

    //automatically find path
    private void AutoPath()
    {
        if (player == null)
        {
            return;
        }
        
        pathGenerateTimer += Time.deltaTime;
        if (pathGenerateTimer >= pathGenerateInterval)
        {
            GeneratePath(player.position);
            pathGenerateTimer = 0f; //reset timer
        }
        
        //if pathpointlist is empty, calculate the path
        if (pathPointList == null || pathPointList.Count <= 0 || pathPointList.Count<=currentIndex) 
        {
            GeneratePath(player.position);
        }
        else if (currentIndex < pathPointList.Count)
        {
            //if enemy arrive to the point, increment the currentIndex and calculate
            if(Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.1f)
            {
                currentIndex++;
                if (currentIndex >= pathPointList.Count)
                {
                    GeneratePath(player.position);
                }
                {
                
                }
            }
        }
        
    }

    private void MeleeAttackAnimEvent()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackDistance, playerLayer);

        foreach (Collider2D hitCollider in hitColliders)
        {
            hitCollider.GetComponent<Character>().TakeDamage(meleeAttackDamage);
        }
    }
    
    private void GeneratePath(Vector3 target)
    {
        currentIndex = 0;
        //starting point, ending point, callback
        seeker.StartPath(transform.position, target, path =>
        {
            pathPointList = path.vectorPath;
        });
    }

    IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(AttackCooldownDuration);
        isAttack = true;
    }
    
    private void OnDrawGizmosSelected()
    {
        //attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        
        
        //chase distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
    
    void AttackPlayer()
    {
        Vector2 direction = player.position - firePoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position + firePoint.up * 0.5f, firePoint.rotation);
        bullet.GetComponent<Bullet>().Initialize(player.position);
    }
}
