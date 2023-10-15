using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WolfController : MonoBehaviour, Damageable
{
    #region Values

    [Header("Status")]
    [SerializeField]
    private float maxHealth; //적 체력
    [SerializeField]
    private float moveSpeed; //이동 속도

    [Header("For Patrolling")] //배회 범위 체크는 마름모 모양의 체크포인트 기준으로 벽이 닿거나, 땅이 안닿으면 반대로감
    [SerializeField]
    private Transform groundCheck_Patrol; 
    [SerializeField]
    private Transform wallCheck_Patrol;
    [SerializeField]
    private float circleRadius;
    [SerializeField]
    LayerMask groundLayer;

    [Header("Jump Attacking")]
    [SerializeField]
    private float jumpHeight; // 점프 높이
    [SerializeField]
    private Transform transform_player; // 플레이어 위치 체크 -> 그 위치로 점프
    [SerializeField]
    private Transform groundCheck_Jump; // 땅에 닿았을 때만 점프
    [SerializeField]
    private Vector2 boxSize;

    [Header("Detect Player")]
    [SerializeField]
    private Vector2 lineOfSite; // 탐지 범위 하이라키창에서 설정할 것
    [SerializeField]
    private LayerMask playerLayer; // 'Player'레이어가 붙은 오브젝트를 감지

    [Header("Other")]
    [SerializeField]
    private Rigidbody2D enemy_Wolf_Rb2d;
    [SerializeField]
    private Animator Anim_Enemy_Wolf;

    private float moveDirection = 1;
    private float currentHealth;
    private bool facingRight = true;
    private bool checkingGround;
    private bool checkingWall;
    private bool isGrounded;
    private bool canSeePlayer;

    public bool HasTakenDamage { get; set; }
    #endregion

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(groundCheck_Patrol.position, circleRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheck_Patrol.position, circleRadius, groundLayer);
        isGrounded = Physics2D.OverlapBox(groundCheck_Jump.position, boxSize, 0, groundLayer);
        canSeePlayer = Physics2D.OverlapBox(transform.position, lineOfSite, 0, playerLayer);

        AnimationController();

        if (!canSeePlayer && isGrounded)
        {
            Patrolling();
        }
    }

    public void Damage(float damageAmount)
    {
        HasTakenDamage = true;

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void Patrolling()
    {
        if(!checkingGround || checkingWall)
        {
            if(facingRight)
            {
                Flip();
            }
            else if(!facingRight)
            {
                Flip();
            }
        }
        enemy_Wolf_Rb2d.velocity = new Vector2(moveSpeed * moveDirection, enemy_Wolf_Rb2d.velocity.y);
    }

    private void JumpAttack()
    {
        float distanceFromPlayer = transform_player.position.x - transform.position.x;

        if(isGrounded)
        {
            enemy_Wolf_Rb2d.AddForce(new Vector2(distanceFromPlayer, jumpHeight), ForceMode2D.Impulse);
        }
    }

    private void FlipTowardsPlayer()
    {
        float playerPosition = transform_player.position.x - transform.position.x;
        
        if(playerPosition < 0 && facingRight)
        {
            Flip();
        }
        else if(playerPosition > 0 && !facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        moveDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void AnimationController()
    {
        Anim_Enemy_Wolf.SetBool("canSeePlayer", canSeePlayer);
        Anim_Enemy_Wolf.SetBool("isGrounded", isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck_Patrol.position, circleRadius);
        Gizmos.DrawWireSphere(wallCheck_Patrol.position, circleRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(groundCheck_Jump.position, boxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, lineOfSite);

    }
}
