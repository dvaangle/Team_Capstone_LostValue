using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WolfController : MonoBehaviour, Damageable
{
    #region Variables

    [Header("Status")]
    [SerializeField]
    private float maxHealth_Wolf; //�� ü��
    [SerializeField]
    private float moveSpeed_Wolf; //�̵� �ӵ�
    [SerializeField]
    private float damage_Wolf;

    [Header("For Patrolling")] //��ȸ ���� üũ�� ������ ����� üũ����Ʈ �������� ���� ��ų�, ���� �ȴ����� �ݴ�ΰ�
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
    private float jumpHeight; // ���� ����
    [SerializeField]
    private float jumpMultiplier; // �����̶�� ������ �� ����
    [SerializeField]
    private Transform groundCheck_Jump; // ���� ����� ���� ����
    [SerializeField]
    private Vector2 boxSize;

    [Header("Detect Player")]
    [SerializeField]
    private Vector2 lineOfSite; // Ž�� ���� ���̶�Űâ���� ������ ��
    [SerializeField]
    private LayerMask playerLayer; // 'Player'���̾ ���� ������Ʈ�� ����

    [Header("Other")]
    [SerializeField]
    private Rigidbody2D enemy_Wolf_Rb2d;
    [SerializeField]
    private Animator Anim_Enemy_Wolf;

    private Vector3 enemy_Wolf_Position;
    private float lineOfSite_Pivot;
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
        currentHealth = maxHealth_Wolf;
        lineOfSite_Pivot = 5.0f;
    }

    private void FixedUpdate()
    {
        enemy_Wolf_Position = transform.position;

        checkingGround = Physics2D.OverlapCircle(groundCheck_Patrol.position, circleRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheck_Patrol.position, circleRadius, groundLayer);
        isGrounded = Physics2D.OverlapBox(groundCheck_Jump.position, boxSize, 0, groundLayer);
        canSeePlayer = Physics2D.OverlapBox(enemy_Wolf_Position + new Vector3(lineOfSite_Pivot, 0, 0), lineOfSite, 0, playerLayer);

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
        enemy_Wolf_Rb2d.velocity = new Vector2(moveSpeed_Wolf * moveDirection, enemy_Wolf_Rb2d.velocity.y);
    }

    private void JumpAttack()
    {
        float distanceFromPlayer_EnemyLeft = PlayerController.instance.playerPosition.x - this.transform.position.x;
        float distanceFormPlayer_PlayerLeft = this.transform.position.x - PlayerController.instance.playerPosition.x;

        float distanceFromPlayer_EnemyLeft_Value = Mathf.Abs(distanceFromPlayer_EnemyLeft);
        float distanceFormPlayer_PlayerLeft_Value = Mathf.Abs(distanceFormPlayer_PlayerLeft);

        if (isGrounded)
        {
            if (PlayerController.instance.playerPosition.x < this.transform.position.x)
            {   
                enemy_Wolf_Rb2d.AddForce(new Vector2(distanceFormPlayer_PlayerLeft_Value * jumpMultiplier * -1, jumpHeight), ForceMode2D.Impulse);
                Debug.Log(PlayerController.instance.playerPosition.x + "," + this.transform.position.x);
                Debug.Log(distanceFormPlayer_PlayerLeft_Value);
            }
            else if (PlayerController.instance.playerPosition.x > this.transform.position.x)
            {
                enemy_Wolf_Rb2d.AddForce(new Vector2(distanceFromPlayer_EnemyLeft_Value * jumpMultiplier, jumpHeight), ForceMode2D.Impulse);
                Debug.Log(distanceFromPlayer_EnemyLeft_Value);
            }
        }
    }

    private void FlipTowardsPlayer()
    {
        float playerPosition = PlayerController.instance.playerPosition.x - this.transform.position.x;
        
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
        lineOfSite_Pivot *= -1;
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
        Gizmos.DrawWireCube(enemy_Wolf_Position + new Vector3(lineOfSite_Pivot, 0, 0), lineOfSite);

    }
}