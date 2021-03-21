using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserProfile : MonoBehaviour
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
        hp = partyData.hpBruiser;
        damage = partyData.damageBruiser;
        armor = partyData.armorBruiser;

    }

    public float getDamage()
    {
        return damage;
    }

    public void attackWithSword()
    {

    }

    public void calculateDamage()
    {

    }

    public void healHimSelf()
    {

    }

    public void activateUlti()
    {

    }
}
