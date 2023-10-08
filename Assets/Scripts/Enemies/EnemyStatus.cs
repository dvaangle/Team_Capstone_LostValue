using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 3f;

    private float currentHealth;
    private void Start()
    {
        currentHealth = maxHealth;
    }
}
