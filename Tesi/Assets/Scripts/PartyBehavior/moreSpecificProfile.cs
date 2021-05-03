using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moreSpecificProfile : aProfile
{
    public PartyData partyData;
    public healthBarScript healthBar;
    protected string champTag;
    protected float armorReductionDuration = 4.0f;
    protected float woundsDuration = 6.0f;

    private bool isDefending;
    private int alive = 0; //0 alive, 1 dead

    private bool m_HitDetect_dash_left;
    private bool m_HitDetect_dash_right;
    private bool m_HitDetect_dash_back;

    private bool m_HitDetect_onRay_front;

    RaycastHit m_Hit_dash_left;
    RaycastHit m_Hit_dash_right;
    RaycastHit m_Hit_dash_back;

    RaycastHit m_Hit_onRay_front;


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
        healthBar.setMaxHealth(currenthp);
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

        healthBar.setHealth(currenthp);
    }

    public void publicAddLifeByCure(float cure)
    {
        addLifeByCure(cure);
    }

    protected override void setLifeAfterDamage(float damage)
    {
        if(shield > 0)
        {
            // NEED TO SCALE DAMAGE BASE ON ARMOR (MUST THINK TO A FORMULA)
            float resultDS = damage - shield;
            if( resultDS > 0)
            {
                resetShield();
                //ricuciamo barra scudo del necessario
                currenthp -= resultDS;
                healthBar.setHealth(currenthp);
            }
            else if ( resultDS < 0)
            {    
                shield = System.Math.Abs(resultDS);
                //ricuciamo barra scudo del necessario
            }
            else
            {
                resetShield();
                //ricuciamo barra scudo del necessario
            }
        }
        else
        {
            // NEED TO SCALE DAMAGE BASE ON ARMOR (MUST THINK TO A FORMULA)
            currenthp -= damage;
            healthBar.setHealth(currenthp);
        }
        

        if (currenthp <=0)
        {
            currenthp = 0;
            healthBar.setHealth(0);

            alive = 1;
            ////DEATH ANIMATION////
            ///DE-ACTIVATION OF REAL CHAMP INSTANTIATION OF DEATH MODEL OR SOMETHING SIMILAR
            
            ///FAR FINIRE EPISODIO SE INVECE E" BOSS O ULTIMO DEL PARTY
            ///
            if (transform.tag.Equals("Boss"))
            {
                //// END EPISODE WITH MALUS ///// 
                Debug.Log("SAREBBE MORTO IL BOSS PROSEGUIAMO"); 
            }
            else
            {
                /// BOSS HAS TO REMOVE THE CHAMP FROM THE ARRAY 
                FindObjectOfType<BossProfile>().checkChampDieInFight();
            }
        }
        //Debug.Log("VITA DOPO ESSERE COLPITO " + currenthp +" DI " + transform.tag);
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

        m_HitDetect_dash_left = Physics.BoxCast(transform.position, transform.localScale, -transform.right, out m_Hit_dash_left, transform.rotation, m_MaxDistance, m_PlayerMask);
        m_HitDetect_dash_right = Physics.BoxCast(transform.position, transform.localScale, transform.right, out m_Hit_dash_right, transform.rotation, m_MaxDistance, m_PlayerMask);
        m_HitDetect_dash_back = Physics.BoxCast(transform.position, transform.localScale, -transform.forward, out m_Hit_dash_back, transform.rotation, m_MaxDistance, m_PlayerMask);

        if (!m_HitDetect_dash_left && !m_HitDetect_dash_right && !m_HitDetect_dash_back)
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
        else if (m_HitDetect_dash_left && !m_HitDetect_dash_right && !m_HitDetect_dash_back)
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
        else if (m_HitDetect_dash_left && m_HitDetect_dash_right && !m_HitDetect_dash_back)
        {
            rb.velocity = -transform.forward * dashForce; //BACK
        }
        else if (!m_HitDetect_dash_left && m_HitDetect_dash_right && !m_HitDetect_dash_back)
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
        else if (!m_HitDetect_dash_left && !m_HitDetect_dash_right && m_HitDetect_dash_back)
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
        else if (m_HitDetect_dash_left && !m_HitDetect_dash_right && m_HitDetect_dash_back)
        {
            rb.velocity = transform.right * dashForce; //RIGHT
        }
        else if (!m_HitDetect_dash_left && m_HitDetect_dash_right && m_HitDetect_dash_back)
        {
            rb.velocity = -transform.right * dashForce; //LEFT
        }
        else
        {
            //CASE HIT ALL DIRECTION IN THEORY NOT POSSIBLE
            Debug.Log("CASO CHE NON DOVREBBE VERIFICARSI DI COLPITE IN TUTTE LE DIREZIONI PER ROLL ");
        }

    }

    public void impulseFromRay(float m_MaxDistance, LayerMask m_PlayerMask)
    {
        m_HitDetect_onRay_front = Physics.BoxCast(transform.position, transform.localScale, transform.forward, out m_Hit_onRay_front, transform.rotation, m_MaxDistance, m_PlayerMask);

        if (m_HitDetect_onRay_front)
        {
            m_Hit_onRay_front.collider.transform.GetComponent<Rigidbody>().AddExplosionForce(34.0f, FindObjectOfType<BossProfile>().transform.position, 15.0f, 2.2F, ForceMode.Impulse);
        }
    }

    public bool publicGetIsDefending()
    {
        return isDefending;
    }

    public void publicSetIsDefending(bool value)
    {
        isDefending = value;
    }

    public int getStatusLifeChamp()
    {
        return alive;
    }

    public void turnToBoss()
    {
        Vector3 verticalAdj = new Vector3(FindObjectOfType<BossProfile>().transform.position.x, transform.position.y, FindObjectOfType<BossProfile>().transform.position.z);
        //Vector3 verticalAdj = new Vector3(originalPositionTarget.x, transform.position.y, originalPositionTarget.z);

        transform.LookAt(verticalAdj);
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_HitDetect_onRay_front)
        {
            Gizmos.DrawRay(transform.position, transform.forward * m_Hit_onRay_front.distance);
            Gizmos.DrawWireCube(transform.position + transform.forward * m_Hit_onRay_front.distance, transform.localScale);
        }
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * 50.0f);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * 50.0f, transform.localScale);
        }
    }
    */


}

