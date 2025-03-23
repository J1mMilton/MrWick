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

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
    }

    private void Update()
    {
        if (player == null)
            return;
        float distance = Vector2.Distance(player.position, transform.position);

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
                OnAttack?.Invoke();
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
    
    private void GeneratePath(Vector3 target)
    {
        currentIndex = 0;
        //starting point, ending point, callback
        seeker.StartPath(transform.position, target, path =>
        {
            pathPointList = path.vectorPath;
        });
    }
}
