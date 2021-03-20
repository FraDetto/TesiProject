using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;
    private GameObject sword;
    private GameObject shield;
    public Transform swordTransform;
    private Rigidbody myRB;

    public PartyData partyData;

    // Start is called before the first frame update
    void Start()
    { 
        hp = partyData.hpTank;
        damage = partyData.damageTank;
        armor = partyData.armorTank;

    }
    
    public void attackWithSword()
    {
        Debug.Log("attackWithSword");
      
    }

    public void calculateDamage()
    {

    }

    public void defendWithShield()
    {

    }

    public void activateUlti()
    {

    }

}
