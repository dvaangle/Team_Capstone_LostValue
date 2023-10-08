using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public static PlayerController instance = null;
    private Rigidbody2D rb2D;
    private Collider2D coll;


    [Header("Movement System")]
    [SerializeField]
    private float moveSpeed; // ������ �ӵ�
    private bool isFacingRight = true;

    [Header("Jump System")]
    [SerializeField]
    private float jumpTime; // t = jumpCounter/jumpTime �� ���̴� ����, �ִ� ���̱��� �����ϴµ����� �ð��� ����
    [SerializeField]
    private int jumpForce; // velocity��
    [SerializeField]
    float fallMultiplier; // ���� ���ӵ��� �����ϴ� ���
    [SerializeField]
    float jumpMultiplier; // ���� ���ӵ��� �����ϴ� ���
    private Vector2 vecGravity;

    [Header("Ground Check")]
    [SerializeField]
    private float extraHeight = 0.25f;
    [SerializeField]
    private LayerMask whatIsGround;


    [Header("Dash System")]
    [SerializeField]
    private float dashVelocity; // �뽬 �ӵ�
    [SerializeField]
    private float MaxDashDuration; // �뽬 ���ӽð�
    [SerializeField]
    private float dashCooltime; // �뽬 ��Ÿ��

    private float moveInput_X;
    private bool canDash = true;
    private bool isDashing;
    private bool isJumping;
    private bool isFalling;
    private float jumpTimeCounter;
    private float dashDuration;
    private RaycastHit2D groundHit;

    #endregion

    void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        coll = gameObject.GetComponent<Collider2D>();
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
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

        if(Input.GetKeyDown(KeyCode.X) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
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
        //��ư�� ������ ��
        if(InputManager.instance.player_InputSettings.Jumping.Jump.WasPressedThisFrame() && isGrounded())
        {
            isJumping = true;
            jumpTimeCounter = 0; 
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
        }

        //��ư�� ������ ���� ��
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

        //��ư�� �� ��
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
        
        rb2D.velocity = new Vector2(0f, 0f); //�뽬 ���� �ణ ����� �Ұ�����
        rb2D.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooltime);
        canDash = true;
    }

    #endregion
}
