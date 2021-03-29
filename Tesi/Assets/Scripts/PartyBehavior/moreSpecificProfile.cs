using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moreSpecificProfile : aProfile
{
    public PartyData partyData;
    protected string champTag;

    [SerializeField] private float totalhp;
    [SerializeField] private float currenthp;
    [SerializeField] private float shieldValue;
    [SerializeField] private float damage;
    [SerializeField] private float armor;


    private void Start()
    {
        champTag = transform.tag;

        switch (champTag)
        {
            case "Tank":
                totalhp = partyData.hpTank;
                currenthp = totalhp;
                damage = partyData.damageTank;
                armor = partyData.armorTank;
                break;

            case "Bruiser":
                totalhp = partyData.hpBruiser;
                currenthp = totalhp;
                damage = partyData.damageBruiser;
                armor = partyData.armorBruiser;
                break;

            case "Mage":
                totalhp = partyData.hpMage;
                currenthp = totalhp;
                damage = partyData.damageMage;
                armor = partyData.armorMage;
                break;

            default:
                totalhp = partyData.hpHealer;
                currenthp = totalhp;
                damage = partyData.damageHealer;
                armor = partyData.armorHealer;
                break;
        }

       
    }

    protected override float getTotalLife()
    {
        return totalhp;
    }

    protected override float getCurrentLife()
    {
        return currenthp;
    }

    protected override float getDamageValue()
    {
        return damage;
    }

    protected override void addLifeByCure(float cure)
    {
        currenthp += cure;
    }

    protected override void setLifeAfterDamage(float damage)
    {
        currenthp -= damage;
    }

}

