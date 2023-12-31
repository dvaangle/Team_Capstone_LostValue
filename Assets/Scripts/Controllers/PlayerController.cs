using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public static PlayerController instance = null;
    public bool ShouldBeDamaging { get; private set; } = false;

    private List<Damageable> damageables = new List<Damageable>();
    private Rigidbody2D rb2D;
    private Collider2D coll;

    [Header("Movement System")]
    [SerializeField]
    private float moveSpeed; // 움직임 속도
    private bool isFacingRight = true;

    [Header("Jump System")]
    [SerializeField]
    private float jumpTime; // t = jumpCounter/jumpTime 에 쓰이는 변수, 최대 높이까지 도달하는데까지 시간에 관여
    [SerializeField]
    private int jumpForce; // velocity값
    [SerializeField]
    float fallMultiplier; // 낙하 가속도에 관여하는 계수
    [SerializeField]
    float jumpMultiplier; // 점프 가속도에 관여하는 계수
    private Vector2 vecGravity;

    [Header("Ground Check")]
    [SerializeField]
    private float extraHeight = 0.25f;
    [SerializeField]
    private LayerMask whatIsGround;


    [Header("Dash System")]
    [SerializeField]
    private float dashVelocity; // 대쉬 속도
    [SerializeField]
    private float MaxDashDuration; // 대쉬 지속시간
    [SerializeField]
    private float dashCooltime; // 대쉬 쿨타임

    [Header("Attack System")]
    [SerializeField]
    private Transform attackTransform;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float damageAmount_NormalAttack;
    [SerializeField]
    private float attackDelay;
    [SerializeField]
    private LayerMask attackableLayer;

    [Header("Player Status")]
    public PlayerStatus health;

    private float moveInput_X;
    private bool canDash = true;
    private bool isDashing;
    private bool isJumping;
    private bool isFalling;
    private float jumpTimeCounter;
    private float dashDuration;
    private float attackTimeCounter;
    private RaycastHit2D groundHit;
    private RaycastHit2D[] hits;
    [HideInInspector]
    public Vector3 playerPosition;

    #endregion

    private void Awake()
    {
        if(PlayerController.instance == null)
        {
            PlayerController.instance = this;
        }
        health.Initialize();
    }

    void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        coll = gameObject.GetComponent<Collider2D>();
        vecGravity = new Vector2(0, -Physics2D.gravity.y);

        //시작하자마자 공격 할수 있게(없으면 게임 시작하고 딜레이 시간동안 공격x)
        attackTimeCounter = attackDelay;

        Debug.Log(isFacingRight);
        StartDirectionCheck();
    }

    void Update()
    {
        if(isDashing)
        {
            return;
        }

        Move();
        Jump();

        if(InputManager.instance.player_InputSettings.Dashing.Dash.IsPressed() && canDash)
        {
            StartCoroutine(Dash());
        }

        if(InputManager.instance.player_InputSettings.Attacking.Attack.WasPressedThisFrame() && attackTimeCounter >= attackDelay)
        {
            //reset counter
            attackTimeCounter = 0f;

            Attack();
        }

        attackTimeCounter += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        playerPosition = gameObject.transform.position;

        if (isDashing)
        {
            return;
        }

        Physics2D.IgnoreLayerCollision(8, 9);
    }

    #region Movement Func
    private void Move()
    {
        moveInput_X = InputManager.instance.moveInput.x;

        if(moveInput_X > 0 || moveInput_X < 0)
        {
            TrunCheck();
        }

        rb2D.velocity = new Vector2(moveInput_X * moveSpeed, rb2D.velocity.y);
    }
    #endregion

    #region TurnCheck
    private void StartDirectionCheck()
    {
        if(moveInput_X > 0)
        {
            isFacingRight = true;
        }
        else
        {
            isFacingRight = false;
        }
    }

    private void TrunCheck()
    {
        if(InputManager.instance.moveInput.x > 0 && !isFacingRight)
        {
            Turn();
        }
        else if(InputManager.instance.moveInput.x < 0 && isFacingRight)
        {
            Turn();
        }
    }
    void Turn()
    {
        if(isFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
        }
    }
    #endregion

    #region Jump
    private void Jump()
    {
        //버튼이 눌렸을 때
        if(InputManager.instance.player_InputSettings.Jumping.Jump.WasPressedThisFrame() && isGrounded())
        {
            isJumping = true;
            jumpTimeCounter = 0; 
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
        }

        //버튼이 눌리는 중일 때
        if(InputManager.instance.player_InputSettings.Jumping.Jump.IsPressed())
        {
            if(jumpTimeCounter >= 0 && jumpTimeCounter < jumpTime && isJumping)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
                jumpTimeCounter += Time.deltaTime;

                float t = jumpTimeCounter / jumpTime;
                float currentJumpMultiplier = jumpMultiplier;

                if (t > 0.5f)
                {
                    currentJumpMultiplier = jumpMultiplier * (1 - t);
                }

                rb2D.velocity += vecGravity * currentJumpMultiplier * Time.deltaTime;
            }
            else if(jumpTimeCounter == jumpTime)
            {
                isFalling = true;
                isJumping = false;

                rb2D.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }

        }

        //버튼을 뗄 때
        if (InputManager.instance.player_InputSettings.Jumping.Jump.WasReleasedThisFrame())
        {
            isJumping = false;
            isFalling = true;
        }

        DrawGroundCheck();
    }

    #endregion

    #region Ground/Landed Check
    bool isGrounded()
    {
        groundHit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, extraHeight, whatIsGround);

        if(groundHit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckForLand()
    {
        if(isFalling)
        {
            if(isGrounded())
            {
                isFalling = false;

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Debug Func
    
    private void DrawGroundCheck()
    {
        Color rayColor;

        if(isGrounded())
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(coll.bounds.center + new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, 0), Vector2.down * (coll.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(coll.bounds.center - new Vector3(coll.bounds.extents.x, coll.bounds.extents.y + extraHeight), Vector2.right * (coll.bounds.extents.x * 2), rayColor);
    }
    #endregion

    #region Dash
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb2D.gravityScale;
        rb2D.gravityScale = 0f;
        for (dashDuration = 0.0f; dashDuration < MaxDashDuration; dashDuration += Time.deltaTime)
        {
            if(isFacingRight)
            {
                rb2D.velocity = new Vector2(dashVelocity, 0f);
            }
            else if(!isFacingRight)
            {
                rb2D.velocity = new Vector2(dashVelocity * -1, 0f);

            }
            yield return null;

            if(dashDuration >= MaxDashDuration)
            {
                dashDuration = 0;
            }
        }
        
        rb2D.velocity = new Vector2(0f, 0f); //대쉬 직후 약간 끊기게 할것인지
        rb2D.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooltime);
        canDash = true;
    }

    #endregion

    private void Attack()
    {
        hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            Damageable damageable = hits[i].collider.gameObject.GetComponent<Damageable>();

            if (damageable != null)
            {
                damageable.Damage(damageAmount_NormalAttack);
            }
        }

        Debug.Log("normal Attack");
    }

    /*    public IEnumerator DamageWhileAttackIsActive()
        {
            ShouldBeDamaging = true;

            while(ShouldBeDamaging)
            {
                hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

                for (int i = 0; i < hits.Length; i++)
                {
                    Damageable damageable = hits[i].collider.gameObject.GetComponent<Damageable>();

                    //if we found an iDamageable
                    if (damageable != null && damageables.Contains(damageable))
                    {
                        //apply damage
                        damageable.Damage(damageAmount);
                        damageables.Add(damageable);
                    }
                }

                yield return null;
            }
            ReturnAttablesToDamageable();

        }

        private void ReturnAttablesToDamageable()
        {
            foreach(Damageable thingThatWasDamaged in damageables)
            {
                thingThatWasDamaged.HasTakenDamage = false;
            }

            damageables.Clear();
        }
    */
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }

    #region Animation Triggers

    public void ShouldBeDamagingToTrue()
    {
        ShouldBeDamaging = true;
    }

    public void ShouldBeDamagingToFalse()
    {
        ShouldBeDamaging = false;
    }

    #endregion
}
