using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStatus
{
    [SerializeField]
    private PlayerInfo_HealthBar playerInfo_HealthBar;
    [SerializeField]
    private float maxValue;
    [SerializeField]
    private float currentValue;

    public float CurrentValue
    {
        get
        {
            return currentValue;
        }
        set
        {
            this.currentValue = value;
            playerInfo_HealthBar.Value = currentValue;
        }
    }
    public float MaxValue
    {
        get
        {
            return maxValue;
        }
        set
        {
            this.maxValue = value;
            playerInfo_HealthBar.MaxValue = value;
        }
    }
    public void Initialize()
    {
        this.MaxValue = maxValue;
        this.CurrentValue = currentValue;
    }
}
