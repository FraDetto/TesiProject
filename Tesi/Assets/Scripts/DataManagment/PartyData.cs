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

    public float hpTank = 300.0f;
    public float damageTank = 40.0f;
    public float armorTank = 15.0f;

    /// <summary>
    /// BRUISER VALUES
    /// </summary>
    /// 
    public float hpBruiser = 220.0f;
    public float damageBruiser = 60.0f;
    public float armorBruiser = 11.0f;

    /// <summary>
    /// MAGE VALUES
    /// </summary>
    /// 
    public float hpMage = 150.0f;
    public float damageMage = 90.0f;
    public float armorMage = 9.0f;

    /// <summary>
    /// HEALER VALUES
    /// </summary>
    /// 
    public float hpHealer = 180.0f;
    public float damageHealer = 25.0f;
    public float armorHealer = 9.0f;

    /// <summary>
    /// BOSS VALUES
    /// </summary>
    /// 
    public float hpBoss = 300.0f;
    public float damageBoss = 40.0f;
    public float armorBoss = 15.0f;

   
}
