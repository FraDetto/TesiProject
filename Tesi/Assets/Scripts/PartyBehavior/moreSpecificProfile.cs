using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moreSpecificProfile : aProfile
{
    public PartyData partyData;
    protected string champTag;
    protected float armorReductionDuration = 4.0f;
    protected float woundsDuration = 6.0f;


    [SerializeField] private float totalhp;
    [SerializeField] private float currenthp;
    [SerializeField] private float shield;
    [SerializeField] private float damage;
    [SerializeField] private float armor;
    [SerializeField] private int status; // 0: OK  1: ROOT  2: STUN 
    [SerializeField] private bool woundsActive;


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

            case "Healer":
                totalhp = partyData.hpHealer;
                currenthp = totalhp;
                damage = partyData.damageHealer;
                armor = partyData.armorHealer;
                break;

            default:
                totalhp = partyData.hpBoss;
                currenthp = totalhp;
                damage = partyData.damageBoss;
                armor = partyData.armorBoss;
                break;
        }

        status = 0;
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
        return getCurrentLife();
    }

    protected override float getDamageValue()
    {
        return damage;
    }

    public float publicGetDamageValue()
    {
        return getDamageValue();
    }

    protected override int getStatus()
    {
        return status;
    }

    public int publicGetStatus()
    {
        return getStatus();
    }

    protected override void addLifeByCure(float cure)
    {
     
        if ( (currenthp + cure)<= totalhp)
        {
            currenthp += cure;
        }
        else
        {
            currenthp = totalhp;
        }
        
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
        setLifeAfterDamage(damage);
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
        yield return new WaitForSeconds(5.0f);
        resetShield();
        Debug.Log("Scudo FINITO");
    }

    protected override void resetShield()
    {
        shield = 0.0f;
    }


    protected override void addRootStatus(float rootDuration)
    {
        status = 1;
        StartCoroutine(decayRoot(rootDuration));
    }

    public void publicAddRootStatus(float rootDuration)
    {
        addRootStatus(rootDuration);
    }

    public void publicSetPreviousStatus(int value)
    {
        status = value;
    }

    public IEnumerator decayRoot(float rootDuration)
    {
        yield return new WaitForSeconds(rootDuration);
        status = 0;
    }

    protected override void addStunStatus(float stunDuration)
    {
        status = 2;
        StartCoroutine(decayStun(stunDuration));
    }


    public void publicAddStunStatus(float stunDuration)
    {
        addStunStatus(stunDuration);
    }

    public IEnumerator decayStun(float stunDuration)
    {
        yield return new WaitForSeconds(stunDuration);
        status = 0;
    }

    public void reduceArmor()
    {
        float originalArmor = armor;
        armor -= (armor / 100 * 30);
        StartCoroutine(resetArmor(armorReductionDuration, originalArmor));
    }

    public IEnumerator resetArmor(float armoreReduceDuration, float originalArmor)
    {
        yield return new WaitForSeconds(armoreReduceDuration);
        armor = originalArmor;
    }

    public bool getWoundActive()
    {
        return woundsActive;
    }

    public void applyWound()
    {
        woundsActive = true;
    }

    public IEnumerator resetWound()
    {
        yield return new WaitForSeconds(woundsDuration);
        woundsActive = false;
    }

}

