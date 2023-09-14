using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2D;
    [Header("Movement System")]
    [SerializeField]
    private float moveSpeed; // 움직임 속도
    private float moveHorizontal;

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

    bool isJumping;
    float jumpCounter;

    public Transform groundCheck; 
    public LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        rb2D = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
            isJumping = true;
            jumpCounter = 0;
        }

        if(rb2D.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.deltaTime;
            if(jumpCounter > jumpTime)
            {
                isJumping = false;
            }

            float t = jumpCounter / jumpTime;
            float currentJumpM = jumpMultiplier;

            if(t > 0.5f)
            {
                currentJumpM = jumpMultiplier * (1 - t);
            }

            rb2D.velocity += vecGravity * currentJumpM * Time.deltaTime;
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            jumpCounter = 0;

            if(rb2D.velocity.y > 0)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y * 0.6f);
            }
        }

        if(rb2D.velocity.y < 0)
        {
            rb2D.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if(moveHorizontal > 0.1f || moveHorizontal < -0.1f)
        {
            rb2D.AddForce(new Vector2(moveHorizontal * moveSpeed, 0f), ForceMode2D.Impulse);
        }

    }

    bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 0.06f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }
}
