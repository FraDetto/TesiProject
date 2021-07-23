using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PartyData", order = 1)]
public class PartyData : ScriptableObject
{
    /// <summary>
    /// TANK VALUES
    /// </summary>
    ///

    public float hpTank = 700.0f;
    public float damageTank = 120.0f;
    public float armorTank = 90.0f;

    /// <summary>
    /// BRUISER VALUES
    /// </summary>
    /// 
    public float hpBruiser = 450.0f;
    public float damageBruiser = 135.0f;
    public float armorBruiser = 70.0f;

    /// <summary>
    /// MAGE VALUES
    /// </summary>
    /// 
    public float hpMage = 300.0f;
    public float damageMage = 155.0f;
    public float armorMage = 45.0f;

    /// <summary>
    /// HEALER VALUES
    /// </summary>
    /// 
    public float hpHealer = 380.0f;
    public float damageHealer = 100.0f;
    public float armorHealer = 55.0f;

    /// <summary>
    /// BOSS VALUES
    /// </summary>
    /// 
    public float hpBoss = 4200.0f;
    public float damageBoss = 200.0f;
    public float armorBoss = 110.0f;

   
}
