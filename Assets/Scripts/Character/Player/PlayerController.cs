using System.Collections;
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
    public int directionFlag = 1; // 1 is right, -1 is left
    public float playerSize;
    private Vector2 direction; // current movement direction
    public bool isDead;
    public bool isHurt;

    // Melee attack cooldown variables.
    public float meleeAttackCooldown = 2f;
    private bool canMeleeAttack = true;
    
    // Sprint cooldown variables.
    public float sprintCooldown = 2f;
    private bool canSprint = true;
    private bool isSprinting = false;
    
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
        inputActions.Player.Sprint.started += OnSprintStarted;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;
        inputActions.Player.Enable();
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.RangeAttack.started -= OnRangeAttack;
        inputActions.Player.MeleeAttack.started -= OnMeleeAttack;
        inputActions.Player.Sprint.started -= OnSprintStarted;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;
        inputActions.Player.Disable();
        inputActions.Disable();
    }

    private void Update()
    {
        direction = inputActions.Player.Move.ReadValue<Vector2>();

        // Flip player based on horizontal input.
        if (direction.x > 0)
            transform.localScale = new Vector3(playerSize, playerSize, playerSize); // facing right
        else if (direction.x < 0)
            transform.localScale = new Vector3(-playerSize, playerSize, playerSize); // facing left

        // Set animator "WalkValue" based on movement and sprint status.
        if (direction.sqrMagnitude > 0)
        {
            if (isSprinting)
                animator.SetFloat("WalkValue", 3);
            else
                animator.SetFloat("WalkValue", 1);
        }
        else
        {
            animator.SetFloat("WalkValue", 0);
        }

        SetAnimation();
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * moveSpeed;
    }

    private void OnRangeAttack(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Fire");
    }

    private void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if (canMeleeAttack)
        {
            animator.SetTrigger("Melee");
            StartCoroutine(MeleeAttackCooldown());
        }
    }

    private IEnumerator MeleeAttackCooldown()
    {
        canMeleeAttack = false;
        yield return new WaitForSeconds(meleeAttackCooldown);
        canMeleeAttack = true;
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        if (canSprint)
        {
            isSprinting = true;
            moveSpeed = 5; // sprint speed
        }
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
        moveSpeed = 3.7f; // normal speed
        StartCoroutine(SprintCooldown());
    }

    private IEnumerator SprintCooldown()
    {
        canSprint = false;
        yield return new WaitForSeconds(sprintCooldown);
        canSprint = true;
    }

    public void PlayerDead()
    {
        isDead = true;
        SwitchActionMap(inputActions.UI);
        StartCoroutine(DelayDieSceneLoad());
    }
    
    private IEnumerator DelayDieSceneLoad()
    {
        yield return new WaitForSeconds(1f);
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
