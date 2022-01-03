using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for a 2.5D game project where player controls a robot .
/// Player moves on x axis and can wall-jump, hit with sword, parry and dash.
/// Mouse controls players head and player can dash to mouse-direction.
/// A-D controls player move-direction.
/// </summary>
public class Player : MonoBehaviour
{
    #region Variables

    public Transform head;

    [SerializeField]
    float rotationSpeed = 2f;

    [SerializeField]
    float walkSpeed = 2f;

    [Header("Attack settings:")]

    [SerializeField]
    int hitDamage = 1;

    [SerializeField]
    int dashDamage = 1;


    [Header("Dash settings:")]
    [SerializeField]
    float dashSpeed = 15f;

    [SerializeField]
    float dashFallMultiplier = 15f;

    [Tooltip("In seconds")]
    [SerializeField]
    float dashDuration = 1f;

    [Tooltip("Lower number = less curve")]
    [SerializeField]
    float dashCurve = 8f;

    [Header("Jump settings:")]
    [SerializeField]
    float jumpForce = 5f;

    [SerializeField]
    float gravity = 19f;

    [Tooltip("Jumping up takes longer than falling down")]
    [SerializeField]
    float fallMultiplier = 2f;

    [SerializeField]
    float lowJumpMultiplier = 1.5f;

    [Header("Parry settings:")]

    [SerializeField]
    float parryDuration = 2f;

    [SerializeField]
    float parryCooldown = 2f;

    [Tooltip("Plays every time when M1")]
    [SerializeField]
    ParticleSystem attackEffect = null;

    [SerializeField]
    ParticleSystem dashEffect = null;

    private Vector3 movement;
    private float cameraZposition;
    private bool canWallJump;
    private Sword sword;
    private PlayerHealth health;
    private float finalRotationAngle;
    private bool canParry = true;
    private CharacterController controller;
    private Animator animator;

    #endregion

    #region Properties
    public static Player Instance { get; private set; }
    public int JumpCount { get; set; }
    public bool CanDash { get; set; }
    public float WalkSpeed => walkSpeed;
    public bool IsGrounded => controller.isGrounded;
    public float DashDuration => dashDuration;
    public int HitDamage => hitDamage;
    public int DashDamage => dashDamage;
    #endregion

    #region Statemachine

    private StateMachine stateMachine;
    public GroundedState groundedState;
    public JumpState jumpState;
    public DashState dashState;
    public FallState fallState;

    #endregion

    private void Awake()
    {
        Instance = this;
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        sword = GetComponentInChildren<Sword>();
        health = GetComponent<PlayerHealth>();
        InitializeStates();
    }

    private void Start()
    {
        // Get camera's distance from player
        if(Camera.main.TryGetComponent(out CameraFollow cameraFollow)) {
            cameraZposition = cameraFollow.CameraZ;
        } else
        {
            Debug.LogWarning("CameraFollow missing from main camera!");
        }
        
    }

    private void Update()
    {
        // Listens to player's input
        stateMachine.CurrentState.HandleInput();

        // Calls all methods in current state
        stateMachine.CurrentState.Execute();

        CollisionCheck();

        // Force player to never move on Z axis:
        if (transform.position.z != 0)
        {
            Vector3 newPosition = transform.position;
            newPosition.z = 0;
            transform.position = newPosition;
        }

        if (Input.GetButtonDown("Hit"))
        {
            Attack();
        }
        else if (Input.GetButtonDown("Parry"))
        {
            if(canParry)
            {
                StartCoroutine(Parry(parryDuration));
            }
            
        }

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // if collision point is a straight wall
        if(!controller.isGrounded && hit.normal.y < 0.01f)
        {
            // If player's sides are colliding
            if ((controller.collisionFlags & CollisionFlags.Sides) != 0 && canWallJump)
            {
                // ENable walljumping if player is facing the wall
                float angle = Vector3.Dot(hit.normal, transform.right);

                if (angle > -1.1f && angle < -0.9f)
                {
                    JumpCount = 1;
                    canWallJump = false;
                }
            
            }
        }
    }

    public void Walk(float horizontal)
    {
        movement = new Vector3(horizontal, -1, 0).normalized;
        movement *= walkSpeed;
        controller.Move(movement * Time.deltaTime);
        
        Vector2 input = new Vector2(horizontal, 0);
        animator.SetFloat(AnimatorParams.movement, input.magnitude);
        //Debug.Log(input.magnitude);
    }

    public void RotateHead()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -cameraZposition;
        Vector3 targetPos = Camera.main.WorldToScreenPoint(head.position);
        mousePos.x -= targetPos.x;
        mousePos.y -= targetPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;


        // BODY TURN:
        // IF WE ARE FACING RIGHT AND MOUSE IS UP
        if (angle > 91 ||  angle < -91 && transform.rotation.eulerAngles.y == 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            // turn body to face left

        }// IF WE ARE FACING LEFT AND MOUSE IS UP
        else if (angle < 90 && angle > -90 && transform.rotation.y != 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            //turn body to face right
        }

        // HEAD TURN:
        if (transform.rotation.eulerAngles.y == 0)
        {
            // if facing right
            
            finalRotationAngle = -angle;
            head.localRotation = Quaternion.Euler(finalRotationAngle, 0f, 0f);
        }
        else if (transform.rotation.eulerAngles.y == 180)
        {
            if(angle >= 90 && angle <=180)
            {
                finalRotationAngle = angle - 180;
            } else if(angle >= -180 && angle <= -90)
            {
                finalRotationAngle = angle + 180;
            }

            head.localRotation = Quaternion.Euler(finalRotationAngle, 0f, 0f);
        }
    }

    public void AddJumpForce()
    {
        // add 1 to jumpCount
        JumpCount++;
        // Adds y velocity when jump button is pressed
        movement.y = jumpForce;
        controller.Move(movement * Time.deltaTime);
    }

    public void JumpPhysics(float horizontal)
    {
        if (JumpCount < 2 && Input.GetButtonDown("Jump"))
        {
            AddJumpForce();
        }

        // When player starts to fall down, add acceleration
        if (movement.y < 0)
        {
            movement.y -= gravity * fallMultiplier * Time.deltaTime;
        } else if(movement.y > 0 && !Input.GetButton("Jump"))
        {
            // when holding space, jump higher
            movement.y -= gravity * lowJumpMultiplier * Time.deltaTime;
        }

        //basic gravity
        else
        {
            movement.y -= gravity * Time.deltaTime;
        }

        movement.x = horizontal * walkSpeed / 1.1f;

        controller.Move(movement * Time.deltaTime);
    }

    public void Fall(float horizontal)
    {
        if (horizontal != 0)
        {
            movement.x = horizontal * walkSpeed / 1.1f;
        }
        movement.y -= gravity * dashFallMultiplier * Time.deltaTime;
        controller.Move(movement * Time.deltaTime);
    }

    public void Dash()
    {
        movement = new Vector3(head.transform.forward.x, head.transform.forward.y, 0);
        movement *= dashCurve;
        controller.Move(movement.normalized * dashSpeed * Time.deltaTime);
    }

    public void CollisionCheck()
    {
        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            JumpCount = 2;
            stateMachine.SetState(fallState);
        }

        if (controller.collisionFlags == CollisionFlags.Above || controller.isGrounded ||
            controller.collisionFlags == CollisionFlags.None)
        {
            // if player is on air, grounded, or only head collides, enable wall jumping
            // (prevents multiple jumps against the same wall)
            canWallJump = true;
        }
    }

    // This is called by pressing M1 (not dash)
    public void Attack()
    {
        SetAnimatorTrigger(AnimatorParams.hit);
        SetSwordAttack(Sword.AttackType.normal);
        PlayParticleEffect(attackEffect);
    }

    IEnumerator Parry(float duration)
    {
        canParry = false;
        health.parrying = true;
        yield return new WaitForSeconds(duration);
        StartCoroutine(AbilityCooldown(parryCooldown));
    }

    IEnumerator AbilityCooldown(float time)
    {
        health.parrying = false;
        yield return new WaitForSeconds(time);
        canParry = true;
    }

    public void SetSwordAttack(Sword.AttackType current)
    {
        if(sword != null)
        {
            sword.currentAttack = current;
        }
    }

    public void SetSwordOnOff(bool value)
    {
        sword.enabled = value;
    }

    private void PlayParticleEffect(ParticleSystem effect)
    {
        if(effect != null)
        {
            effect.Play();
        } else
        {
            Debug.LogWarning("Particle Effect not assigned!");
        }
    }

    public void DashEffect()
    {
        PlayParticleEffect(dashEffect);
    }

    private void InitializeStates()
    {
        stateMachine = new StateMachine();
        groundedState = new GroundedState(stateMachine, this);
        jumpState = new JumpState(stateMachine, this);
        dashState = new DashState(stateMachine, this);
        fallState = new FallState(stateMachine, this);
        // start from being grounded
        stateMachine.Initialize(groundedState);
    }

    #region AnimatorMethods
    public void CheckRunDirection(float horizontal)
    {
        if(horizontal > 0 && head.transform.forward.x < 0)
        {
            // facing and running to different direction
            SetAnimatorBool(AnimatorParams.backwards, true);

        } else if(horizontal < 0 && head.transform.forward.x > 0)
        {
            // facing and running to different direction
            SetAnimatorBool(AnimatorParams.backwards, true);
        } else
        {
            // facing and running to same direction
            SetAnimatorBool(AnimatorParams.backwards, false);
        }
    }

    public void SetAnimatorBool(int param, bool value)
    {
        animator.SetBool(param, value);
    }
    public void SetAnimatorFloat(int param, float value)
    {
        animator.SetFloat(param, value);
    }
    public void SetAnimatorTrigger(int param)
    {
        animator.SetTrigger(param);
    }
    public void SetAnimationInt(int param, int value)
    {
        animator.SetInteger(param, value);
    }
    #endregion

}
