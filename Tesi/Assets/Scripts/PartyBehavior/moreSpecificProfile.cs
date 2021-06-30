using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moreSpecificProfile : aProfile
{
    public PartyData partyData;
    public healthBarScript healthBar;
    public shieldBarScript shieldBar;

    public bool flagResetepisode;

    private GameObject bossRef;

    protected string champTag;
    protected float armorReductionDuration = 4.0f;
    protected float woundsDuration = 6.0f;

    private bool isDefending;
    private int alive = 0; //0 alive, 1 dead

    private int typeCode; //0 TANK, 1 BRUISER, 2 MAGE, 3 HEALER

    private bool usedDef;

    private bool shooting;

    private bool m_HitDetect_dash_left;
    private bool m_HitDetect_dash_right;
    private bool m_HitDetect_dash_back;

    private bool m_HitDetect_onRay_front;


    private bool m_HitDetect_swing_right;
    private bool m_HitDetect_swing_left;

    RaycastHit m_Hit_swing_right;
    RaycastHit m_Hit_swing_left;


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

                typeCode = 0;
                break;

            case "Bruiser":
                totalhp = partyData.hpBruiser;
                currenthp = totalhp;
                damage = partyData.damageBruiser;
                armor = partyData.armorBruiser;

                typeCode = 1;
                break;

            case "Mage":
                totalhp = partyData.hpMage;
                currenthp = totalhp;
                damage = partyData.damageMage;
                armor = partyData.armorMage;

                typeCode = 2;
                break;

            case "Healer":
                totalhp = partyData.hpHealer;
                currenthp = totalhp;
                damage = partyData.damageHealer;
                armor = partyData.armorHealer;

                typeCode = 3;
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
        shield = 0.0f;

        healthBar.setMaxHealth(currenthp);
        shieldBar.setMaxShield(shield);
        /*if (!champTag.Equals("Boss"))
        {
            
        }*/

        flagResetepisode = false;
    }

    
    public void setFlaResetEpisode(bool value)
    {
        flagResetepisode = value;
    }

    public int getTypeCode()
    {
        return typeCode;
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

    public bool getShooting()
    {
        return shooting;
    }

    public void setShooting(bool value)
    {
        shooting = value;
    }

    protected override void addLifeByCure(float cure)
    {
        float newHp = currenthp;
        if ( (newHp + cure)<= totalhp)
        {
            newHp += cure;
        }
        else
        {
            newHp = totalhp;
        }
        if (alive == 0)//if alive
        {
            healthBar.setHealth(newHp);
        }
        
    }

    public void publicAddLifeByCure(float cure)
    {
        addLifeByCure(cure);
    }

    public float getArmor()
    {
        return armor;
    }


    protected override void setLifeAfterDamage(float damage)
    {
        float effectiveDamage = damage * (100 / (100 + armor)); //formula for scale damage with armor 

        // only for Moving Training
        if (this.gameObject.tag.Equals("Boss"))
        {
            effectiveDamage = 0;
        }

        if(shield > 0)
        {
            // NEED TO SCALE DAMAGE BASE ON ARMOR (MUST THINK TO A FORMULA)
            float resultDS = effectiveDamage - shield;
            if( resultDS > 0)
            {
                resetShield();

                shieldBar.setShield(shield);
                currenthp -= resultDS;
                healthBar.setHealth(currenthp);
            }
            else if ( resultDS < 0)
            {    
                shield = System.Math.Abs(resultDS);

                shieldBar.setShield(shield);
            }
            else
            {
                resetShield();

                shieldBar.setShield(shield);
            }
        }
        else
        {
            // NEED TO SCALE DAMAGE BASE ON ARMOR (MUST THINK TO A FORMULA)
            currenthp -= effectiveDamage;
            healthBar.setHealth(currenthp);
        }
        

        if (currenthp <=0)
        {
            currenthp = 0;
            healthBar.setHealth(0);

            alive = 1;

            ///DE-ACTIVATION OF REAL CHAMP INSTANTIATION OF DEATH MODEL OR SOMETHING SIMILAR
            
            ///FAR FINIRE EPISODIO SE INVECE E" BOSS O ULTIMO DEL PARTY
            ///
            if (transform.tag.Equals("Boss"))
            {
                //Debug.Log("SAREBBE MORTO IL BOSS PROSEGUIAMO");
                //// END EPISODE WITH MALUS ///// 
                //GetComponent<BossAttackBehavior>().bossDeath();
                GetComponentInChildren<BossBehavior>().bossDeath();

            }
            else
            {
                //this.gameObject.layer = 0; //dead champ --> layer from player to Default to avoid problem with raycast
                isDefending = false;
                usedDef = false;
                /// BOSS HAS TO REMOVE THE CHAMP FROM THE ARRAY 
                bossRef.GetComponentInChildren<BossBehavior>().checkChampDieInFight();

            }
        }
        //Debug.Log("VITA DOPO ESSERE COLPITO " + currenthp +" DI " + transform.tag);
    }

    public void setShieldForBoss(int s)
    {
        shield = s;
        shieldBar.setMaxShield(shield);
    }

    public void publicSetLifeAfterDamage(float damage)
    {
        setLifeAfterDamage(damage);
    }

    protected override void addShield(float shieldValue)
    {
        shield += shieldValue;
        shieldBar.setMaxShield(shield);
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
        shieldBar.setMaxShield(shield);
        //Debug.Log("Scudo FINITO");
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


    public void incrementStatsUltiTank()
    {
        float hpToAdd = (currenthp / 100) * 40;

        float armorToAdd = (armor / 100) * 50;

        currenthp += hpToAdd;
        healthBar.setHealth(currenthp);
        healthBar.setMaxHealth(totalhp+hpToAdd);

        armor += armorToAdd;
        

    }

    public void resetStatsTankUlti(float previousHp, float previousArmor)
    {
        float armorToSubtract = (previousArmor / 100) * 50;
        armor -= armorToSubtract;

        float hpToSubtract = (previousHp / 100) * 40;
        currenthp -= hpToSubtract;
        healthBar.setHealth(currenthp);
        healthBar.setMaxHealth(totalhp - hpToSubtract);
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
            m_Hit_onRay_front.collider.transform.GetComponent<Rigidbody>().AddExplosionForce(34.0f, bossRef.transform.position, 15.0f, 2.2F, ForceMode.Impulse);
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

    public void turnToBoss(GameObject boss)
    {
        Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
        //Vector3 verticalAdj = new Vector3(originalPositionTarget.x, transform.position.y, originalPositionTarget.z);

        transform.LookAt(verticalAdj);
    }

    public void resetBossStats()
    {
        totalhp = partyData.hpBoss;
        currenthp = totalhp;
        damage = partyData.damageBoss;
        armor = partyData.armorBoss;
        healthBar.setMaxHealth(currenthp);

        status = 0;
        shield = 0.0f;
        alive = 0;

        
        flagResetepisode = false;
    }

    public bool getDefUsed()
    {
        return usedDef;
    }

    public void setDefUsed(bool value)
    {
        usedDef = value;
    }

    public void detonation()
    {
        flagResetepisode = true;


        Destroy(this.gameObject);
    }


    public void setBossRef(GameObject bo)
    {
        bossRef = bo;
    }


    public bool swingRayCastControll(GameObject swordSwingAttk, LayerMask m_PlayerMask)//if at least one other champ in the swing's space return true
    {
        m_HitDetect_swing_right = Physics.BoxCast(transform.position, swordSwingAttk.transform.localScale, transform.right, out m_Hit_swing_right, transform.rotation, swordSwingAttk.transform.localScale.x, m_PlayerMask);
        m_HitDetect_swing_left = Physics.BoxCast(transform.position, swordSwingAttk.transform.localScale, -transform.right, out m_Hit_swing_left, transform.rotation, swordSwingAttk.transform.localScale.x, m_PlayerMask);

        if (!m_HitDetect_swing_right && !m_HitDetect_swing_left)
        {
            //Debug.Log("SWING CONTROLLO NO HIT " + swordSwingAttk.transform.localScale.x);
            return false;
        }
        else
        {
            if (m_HitDetect_swing_right)
            {
                if (m_Hit_swing_right.collider.gameObject.GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0)
                {
                    //Debug.Log("SWING CONTROLLO HIT DESTRA");
                    return true;
                }
            }

            if (m_HitDetect_swing_left)
            {
                if (m_Hit_swing_left.collider.gameObject.GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0)//see if the ray hit is an alive champ
                {
                    //Debug.Log("SWING CONTROLLO HIT SINISTRA");
                    return true;
                }
            }
            //Debug.Log("SWING CONTROLLO NO HIT PERCHE KO");
            return false;

        }
    }


    /*
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (m_HitDetect_swing_right)
        {
            Gizmos.DrawRay(transform.position, transform.right * m_Hit_swing_right.distance);
            Gizmos.DrawWireCube(transform.position + transform.right * m_Hit_swing_right.distance, transform.localScale);
        }
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.right * 7f);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.right * 7f, transform.localScale);
        }
    }
    */


}

