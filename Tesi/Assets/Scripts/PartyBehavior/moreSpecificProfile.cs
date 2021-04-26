using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moreSpecificProfile : aProfile
{
    public PartyData partyData;
    protected string champTag;
    protected float armorReductionDuration = 4.0f;
    protected float woundsDuration = 6.0f;

    private bool m_HitDetect_left;
    private bool m_HitDetect_right;
    private bool m_HitDetect_back;

    RaycastHit m_Hit_left;
    RaycastHit m_Hit_right;
    RaycastHit m_Hit_back;


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

            case "Boss":
                totalhp = partyData.hpBoss;
                currenthp = totalhp;
                damage = partyData.damageBoss;
                armor = partyData.armorBoss;
                break;

            default:
                Debug.Log("CHARACTER UNKNOWN");
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
        Debug.Log("VITA DOPO ESSERE COLPITO " + currenthp +" DI " + transform.tag);
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



    public void rollAwayChamp(Rigidbody rb, float m_MaxDistance, LayerMask m_PlayerMask, float dashForce)
    {

        m_HitDetect_left = Physics.BoxCast(transform.position, transform.localScale, -transform.right, out m_Hit_left, transform.rotation, m_MaxDistance, m_PlayerMask);
        m_HitDetect_right = Physics.BoxCast(transform.position, transform.localScale, transform.right, out m_Hit_right, transform.rotation, m_MaxDistance, m_PlayerMask);
        m_HitDetect_back = Physics.BoxCast(transform.position, transform.localScale, -transform.forward, out m_Hit_back, transform.rotation, m_MaxDistance, m_PlayerMask);

        if (!m_HitDetect_left && !m_HitDetect_right && !m_HitDetect_back)
        {
            //Debug.Log("LIBERE TUTTE E TRE");
            int way = Random.Range(1, 4);// 1 left, 2 back, 3 right

            switch (way)
            {
                case 1:
                    rb.velocity = -transform.right * dashForce;
                    break;
                case 2:
                    rb.velocity = -transform.forward * dashForce;
                    break;
                default:
                    rb.velocity = transform.right * dashForce;
                    break;
            }
        }
        else if (m_HitDetect_left && !m_HitDetect_right && !m_HitDetect_back)
        {
            //Debug.Log("CONTATTO L");
            int way = Random.Range(1, 3);// 1 back, 2 right

            switch (way)
            {
                case 1:
                    rb.velocity = -transform.forward * dashForce;
                    break;
                default:
                    rb.velocity = transform.right * dashForce;
                    break;
            }
        }
        else if (m_HitDetect_left && m_HitDetect_right && !m_HitDetect_back)
        {
            rb.velocity = -transform.forward * dashForce; //BACK
        }
        else if (!m_HitDetect_left && m_HitDetect_right && !m_HitDetect_back)
        {
            //Debug.Log("CONTATTO R");
            int way = Random.Range(1, 3);// 1 left, 2 back, 

            switch (way)
            {
                case 1:
                    rb.velocity = -transform.right * dashForce;
                    break;
                default:
                    rb.velocity = -transform.forward * dashForce;
                    break;

            }
        }
        else if (!m_HitDetect_left && !m_HitDetect_right && m_HitDetect_back)
        {
            //Debug.Log("CONTATTO B");
            int way = Random.Range(1, 3);// 1 left, 2 right, 

            switch (way)
            {
                case 1:
                    rb.velocity = -transform.right * dashForce;
                    break;
                default:
                    rb.velocity = transform.right * dashForce;
                    break;
            }

        }
        else if (m_HitDetect_left && !m_HitDetect_right && m_HitDetect_back)
        {
            rb.velocity = transform.right * dashForce; //RIGHT
        }
        else if (!m_HitDetect_left && m_HitDetect_right && m_HitDetect_back)
        {
            rb.velocity = -transform.right * dashForce; //LEFT
        }
        else
        {
            //CASE HIT ALL DIRECTION IN THEORY NOT POSSIBLE
            Debug.Log("CASO CHE NON DOVREBBE VERIFICARSI DI COLPITE IN TUTTE LE DIREZIONI PER ROLL ");
        }

    }
    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (m_HitDetect_left)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * m_Hit_left.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * m_Hit_left.distance, transform.localScale);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, -transform.right * 10.0f);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + -transform.right * 10.0f, transform.localScale);
        }

        if (m_HitDetect_right)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.right * m_Hit_right.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.right * m_Hit_right.distance, transform.localScale);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.right * 10.0f);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.right * 10.0f, transform.localScale);
        }

        if (m_HitDetect_back)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, -transform.forward * m_Hit_back.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + -transform.forward * m_Hit_back.distance, transform.localScale);

        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, -transform.forward * 10.0f);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + -transform.forward * 10.0f, transform.localScale);
        }
    }
    */

}

