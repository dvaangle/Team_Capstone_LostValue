using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_PangolinController : MonoBehaviour, Damageable
{
    #region Variables

    [Header("Status")]
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private float moveSpeed_Pangolin;

    [Header("For Patrolling")] //배회 범위 체크는 마름모 모양의 체크포인트 기준으로 벽이 닿거나, 땅이 안닿으면 반대로감
    [SerializeField]
    private Transform groundCheck_Patrol;
    [SerializeField]
    private Transform wallCheck_Patrol;
    [SerializeField]
    private float circleRadius;
    [SerializeField]
    LayerMask groundLayer;

    [Header("Detect Player")]
    [SerializeField]
    private Vector2 lineOfSite;
    [SerializeField]
    private LayerMask playerLayer;

    [Header("Other")]
    [SerializeField]
    private Rigidbody2D enemy_Pangolin_Rb2d;

    private Vector3 enemy_Pangolin_Position;
    private float lineOfSite_Pivot;
    private float currentHealth;
    private float moveDirection = 1;
    private bool checkingGround;
    private bool checkingWall;
    private bool isGrounded;
    private bool canSeePlayer;
    private bool facingRight = true;

    public bool HasTakenDamage { get; set; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        lineOfSite_Pivot = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        enemy_Pangolin_Position = transform.position;

        checkingGround = Physics2D.OverlapCircle(groundCheck_Patrol.position, circleRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheck_Patrol.position, circleRadius, groundLayer);
        canSeePlayer = Physics2D.OverlapBox(enemy_Pangolin_Position + new Vector3(lineOfSite_Pivot, 0, 0), lineOfSite, 0, playerLayer);
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0)
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
        enemy_Pangolin_Rb2d.velocity = new Vector2(moveSpeed_Pangolin * moveDirection, enemy_Pangolin_Rb2d.velocity.y);
    }

    private void Flip()
    {
        moveDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck_Patrol.position, circleRadius);
        Gizmos.DrawWireSphere(wallCheck_Patrol.position, circleRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(enemy_Pangolin_Position + new Vector3(lineOfSite_Pivot, 0, 0), lineOfSite);
    }
}
