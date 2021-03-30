using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moreSpecificProfile : aProfile
{
    public PartyData partyData;
    protected string champTag;

    [SerializeField] private float totalhp;
    [SerializeField] private float currenthp;
    [SerializeField] private float shield;
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

    public float publicGetTotalLife()
    {
        return getTotalLife();
    }

    protected override float getCurrentLife()
    {
        return currenthp;
    }

    public float publicGetCurrentLife()
    {
        return getTotalLife();
    }

    protected override float getDamageValue()
    {
        return damage;
    }

    public float publicGetDamageValue()
    {
        return getTotalLife();
    }

    protected override void addLifeByCure(float cure)
    {
        currenthp += cure;
    }

    public void publicAddLifeByCure(float cure)
    {
        addLifeByCure(cure);
    }

    protected override void setLifeAfterDamage(float damage)
    {
        currenthp -= damage;
    }

    public void publicSetLifeAfterDamage(float damage)
    {
        addLifeByCure(damage);
    }

    protected override void addShield(float shieldValue)
    {
        shield += shieldValue;
        StartCoroutine(decayShield());
    }

    public void publicAddShield(float shieldValue)
    {
        addShield(shieldValue);
    }

    public IEnumerator decayShield()
    {
        //Debug.Log("ULTI IN COOLDOWN");
        yield return new WaitForSeconds(5.0f);
        Debug.Log("Scudo FINITO");
    }

    protected override void resetShield()
    {
        shield = 0.0f;
    }

}

