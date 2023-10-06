using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variable
    public static PlayerController instance = null;
    private Rigidbody2D rb2D;


    [Header("Movement System")]
    [SerializeField]
    private float moveSpeed; // ������ �ӵ�
    private float moveHorizontal;
    private bool facingRight = true;

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

    [Header("Dash System")]
    [SerializeField]
    private float dashVelocity; // �뽬 �ӵ�
    [SerializeField]
    private float MaxDashDuration; // �뽬 ���ӽð�
    [SerializeField]
    private float dashCooltime; // �뽬 ��Ÿ��

    private bool canDash = true;
    private bool isDashing;
    private bool isJumping;
    private float jumpCounter;
    private float dashDuration;

    public Transform groundCheck;
    public LayerMask groundLayer;

    #endregion

    void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
    }

    void Update()
    {
        if(isDashing)
        {
            return;
        }

        moveHorizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
            isJumping = true;
            jumpCounter = 0;
        }

        if (rb2D.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.deltaTime;
            if (jumpCounter > jumpTime)
            {
                isJumping = false;
            }

            float t = jumpCounter / jumpTime;
            float currentJumpM = jumpMultiplier;

            if (t > 0.5f)
            {
                currentJumpM = jumpMultiplier * (1 - t);
            }

            rb2D.velocity += vecGravity * currentJumpM * Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            jumpCounter = 0;

            if (rb2D.velocity.y > 0)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y * 0.6f);
            }
        }

        if (rb2D.velocity.y < 0)
        {
            rb2D.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
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

        if (moveHorizontal > 0.1f || moveHorizontal < -0.1f)
        {
            rb2D.AddForce(new Vector2(moveHorizontal * moveSpeed, 0f), ForceMode2D.Impulse);
        }
        if (moveHorizontal > 0 && !facingRight)
        {
            Flip();
        }
        if (moveHorizontal < 0 && facingRight)
        {
            Flip();
        }
    }

    bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.05f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb2D.gravityScale;
        rb2D.gravityScale = 0f;
        for (dashDuration = 0.0f; dashDuration < MaxDashDuration; dashDuration += Time.deltaTime)
        {
            rb2D.velocity = new Vector2(transform.localScale.x * dashVelocity, 0f);
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
}
