using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public InputActions inputActions;
    
    public Animator animator;

    public Rigidbody2D rb;

    public float moveSpeed;

    public int directionFlag = 1; //1 is right, -1 is left

    public float playerSize;

    private Vector2 direction; //direction vector

    public bool isDead;
    public bool isHurt;

    private void Awake()
    {
        inputActions = new InputActions();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputActions.Player.RangeAttack.started += OnRangeAttack;
        inputActions.Player.MeleeAttack.started += OnMeleeAttack;
        inputActions.Player.Enable();
        inputActions.Enable();
        
    }

    private void OnDisable()
    {
        inputActions.Player.RangeAttack.started -= OnRangeAttack;
        inputActions.Player.MeleeAttack.started -= OnMeleeAttack;
        inputActions.Player.Disable();
        inputActions.Disable();
    }

    private void Update()
    {
        direction = inputActions.Player.Move.ReadValue<Vector2>();
        bool isSprinting = inputActions.Player.Sprint.ReadValue<float>() > 0f;

        if (direction.x > 0)
        {
            transform.localScale = new Vector3(playerSize, playerSize, playerSize); //turn right
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-playerSize, playerSize, playerSize); //turn left
        }

        if (direction.sqrMagnitude > 0) //if player's moving
        {
            if (isSprinting)
            {
                animator.SetFloat("WalkValue", 3);
                moveSpeed = 5;
            }
            else
            {
                animator.SetFloat("WalkValue", 1);
                moveSpeed = 3.7f;
            }
        }
        else // player is idle
        {
            animator.SetFloat("WalkValue", 0);
            
        }
        
        SetAnimation();
        
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * moveSpeed;
    }
    
    private void OnRangeAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        animator.SetTrigger("Fire");
    }
    
    private void OnMeleeAttack(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        animator.SetTrigger("Melee");
    }

    public void PlayerDead()
    {
        isDead = true;
        //no movement after death
        SwitchActionMap(inputActions.UI);
        StartCoroutine(DelayDieSceneLoad());
    }
    
    IEnumerator DelayDieSceneLoad()
    {
        yield return new WaitForSeconds(1f); // wait 1 seconds
        UnityEngine.SceneManagement.SceneManager.LoadScene("FailureScene");
    }

    
    public void PlayerHurt()
    {
        isHurt = true;
        animator.SetTrigger("Hurt");
    }

    void SetAnimation()
    {
        animator.SetBool("isDead", isDead);
    }

    void SwitchActionMap(InputActionMap actionMap)
    {
        inputActions.Disable();
        actionMap.Enable();
    }
}
