using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    public void Damage(float damageAmount);

    public bool HasTakenDamage { get; set; }
}
