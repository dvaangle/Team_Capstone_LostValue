using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStat_Player : MonoBehaviour
{
    public List<BonusStat_GoodEvil> BaseStatAdditives_GoodEvil { get; set; }
    
    public int BaseValue { get; set; }
    public string StatName { get; set; }
    public string StatDescription { get; set; }
    public int FinalValue { get; set; }

    public BaseStat_Player(int baseValue, string statName, string statDescription)
    {
        this.BaseStatAdditives_GoodEvil = new List<BonusStat_GoodEvil>();
        this.BaseValue = baseValue;
        this.StatName = statName;
        this.StatDescription = statDescription;
    }
    public void AddStatBonus(BonusStat_GoodEvil bonusStat_GoodEvil)
    {
        this.BaseStatAdditives_GoodEvil.Add(bonusStat_GoodEvil);
    }
    public void RemoveStatBonus()
    {

    }
}
