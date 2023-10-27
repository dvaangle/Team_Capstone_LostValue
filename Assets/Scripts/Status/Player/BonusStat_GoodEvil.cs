using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusStat_GoodEvil
{
    public int BonusValue { get; set; }

    public void StatBonus(int bonusValue)
    {
        this.BonusValue = bonusValue;
        Debug.Log("new StatBonus");
    }
}
