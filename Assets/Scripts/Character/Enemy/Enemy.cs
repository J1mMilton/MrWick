using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;
using UnityEngine.SceneManagement;

public class Enemy : Character
{
    public UnityEvent<Vector2> OnMovementInput;

    public UnityEvent OnAttack;
    
    [SerializeField]private Transform player;
    [SerializeField] private float chaseDistance = 3f;
    [SerializeField] private float attackDistance = 0.8f;
    
    private float wanderTimer = 0f;
    private float wanderInterval = 2f;
    private Vector2 wanderDirection;


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

    private float fireRange = 10f;

   
    public GameObject projectilePrefab;
    public float fireCooldown = 2f;
    private float nextFireTime = 0f;

    public Transform barrel;// where to shoot from

    public Animator anim;
    
    
    private Vector3 initialBarrelLocalPosition;

    private EnemyController controller;
    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponent<EnemyController>();
        initialBarrelLocalPosition = barrel.localPosition; // store original position
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Scene_World_02")
        {
            // speed *= 1.5f;
            // fireRate *= 1.2f;
            // usePathfinding = true;
        }

    }

    private void Update()
    {
        if (controller.isDead) return;
        
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
            //wander
            Wander();
        }

        if (Vector3.Distance(transform.position, player.position) <= fireRange && Vector3.Distance(transform.position, player.position) > attackDistance )
        {
            AttackPlayer();
        }
        
    }

    void Wander()
    {
        wanderTimer += Time.deltaTime;

        // Pick a new direction every few seconds
        if (wanderTimer >= wanderInterval)
        {
            wanderTimer = 0f;

            // Pick a random normalized direction
            wanderDirection = new Vector2(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f)
            ).normalized;
        }

        OnMovementInput?.Invoke(wanderDirection);
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
        if (Time.time >= nextFireTime)
        {
            
            anim.SetTrigger("Fire");
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void Shoot()
    {
        Vector2 direction = (player.position - barrel.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);
        GameObject bullet = Instantiate(projectilePrefab, barrel.position, bulletRotation);
        bullet.GetComponent<Bullet>().Initialize(player.position);
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    
}
