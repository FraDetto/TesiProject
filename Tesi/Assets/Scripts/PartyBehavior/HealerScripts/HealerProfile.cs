using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;
    private GameObject sword;
    private GameObject shield;
    private Rigidbody myRB;

    public PartyData partyData;

    // Start is called before the first frame update
    void Start()
    {
        hp = partyData.hpHealer;
        damage = partyData.damageHealer;
        armor = partyData.armorHealer;

    }

    public void attackWithMagic()
    {
        Debug.Log("attackWithMagic");

    }

    public void calculateDamage()
    {

    }

    public void healAlly()
    {

    }

    public void activateUlti()
    {

    }
}
