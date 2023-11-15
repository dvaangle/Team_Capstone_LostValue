using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_PlantController : MonoBehaviour, Damageable
{
    #region Variables

    [Header("Status")]
    [SerializeField]
    private float maxHealth = 3f;

    [Header("Detect Player")]
    [SerializeField]
    private float detectRange;
    [SerializeField]
    private LayerMask playerLayer; // 'Player'레이어가 붙은 오브젝트를 감지

    [Header("Shooting To Player")]
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform bulletPos;

    public bool HasTakenDamage { get; set; }

    private float currentHealth;
    private float timer;
    private bool canSeePlayer;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        canSeePlayer = false;
        currentHealth = maxHealth;
        canSeePlayer = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
    }

    void Update()
    {
        if(canSeePlayer)
        {
            timer += Time.deltaTime;

            if(timer > 2)
            {
                timer = 0;
                Shoot();
            }
        }
        else if(!canSeePlayer)
        {
            timer = 0;
        }
    }

    private void FixedUpdate()
    {
        canSeePlayer = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
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

    private void Shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
