using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageProfile : moreSpecificProfile
{

    //public bool shooting = false;
    private bool cooldown = false;

    public bool cooldownDefense = false;
    public bool cooldownSpecial = true;
    public bool defenseActive = false;
    public bool chargingUlt = false;

    public bool cooldownDash = false;
    public bool isDashing = false;

    public bool rightDefSpellDirection = false;


    private GameObject boss;
    public Transform pointSpawnFireBall;
    public Transform pointSpawnUlt;
    private Rigidbody rb;
    private GameObject go;

    private Vector3 scaleChange;

    public  GameObject fireBall;
    public GameObject defenseSpellSign;


    public float speedSpells = 25.0f;

    private float timeCoolDownMagicAttack = 2.0f;
    private float defenseSpellDuration = 2.0f;
    private float timeCoolDownDefenseSpell = 10.0f;
    private float specialDuration = 3.0f;
    private float timeCoolDownSpecial = 16.0f;

    private float timeRollCooldown = 2.0f;

    private float dashForce = 17.0f;

    float m_MaxDistance = 10.0f;
    public LayerMask m_PlayerMask;

    // Start is called before the first frame update
    void Start()
    {
        //pointSpawnFireBall = transform.GetChild(1);
        //pointSpawnUlt = transform.GetChild(2);

        //boss = GameObject.FindGameObjectWithTag("Boss");
        scaleChange = new Vector3(0.1f, 0.1f, 0.1f);

        rb = GetComponent<Rigidbody>();

        StartCoroutine(waitAfterUlti());
    }


    private void FixedUpdate()
    {
        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            if (GetComponent<moreSpecificProfile>().getShooting())
            {
                go.transform.LookAt(boss.transform.position);
                go.transform.position += go.transform.forward * speedSpells * Time.deltaTime;
            }

            if (chargingUlt)
            {
                if (null!=go)//to catch a destroy problem
                {
                    go.transform.localScale += scaleChange * Time.deltaTime;
                }
                
            }
        }
        else
        {
            if(null != go)
            {
                Destroy(go.gameObject);
            }
            
        }
       
    }



    public void attackWithMagic()
    {
        //Debug.Log("attackWithMagic");
        if (!cooldown)
        {
            if (!GetComponent<moreSpecificProfile>().flagResetepisode)
            {
                GetComponent<moreSpecificProfile>().turnToBoss(boss);
                /*Debug.Log("MAGE ATTACK 1 FIREBALL " + fireBall);
                Debug.Log("MAGE ATTACK 2 FIREBALL " + pointSpawnFireBall.position);
                Debug.Log("MAGE ATTACK 3 FIREBALL " + transform.rotation);
                Debug.Log("MAGE ATTACK 4 FIREBALL " + gameObject.transform);*/
                go = Instantiate(fireBall, pointSpawnFireBall.position, transform.rotation, gameObject.transform);
                //shooting = true;
                GetComponent<moreSpecificProfile>().setShooting(true);
                cooldown = true;
                StartCoroutine(waitAfterAttack());
            }
           
        }
    }

    public IEnumerator waitAfterAttack()
    {
        yield return new WaitForSeconds(timeCoolDownMagicAttack);
        cooldown = false;  
    }


    public void defendWithSpell()
    {
        GetComponent<moreSpecificProfile>().publicSetIsDefending(true);
        GetComponent<moreSpecificProfile>().setDefUsed(true);

        go = Instantiate(defenseSpellSign, pointSpawnUlt.position, transform.rotation, gameObject.transform);
        cooldownDefense = true;
        defenseActive = true;

        int direction = Random.Range(0, 2);

        if(direction == 0)
        {
            rightDefSpellDirection = true;
        }
        else
        {
            rightDefSpellDirection = false;
        }
       

        StartCoroutine(waitBeforeRemoveShield());
        StartCoroutine(cooldownSpellDefense());
    }

    public IEnumerator waitBeforeRemoveShield()
    {
        yield return new WaitForSeconds(defenseSpellDuration);
        defenseActive = false;
        Destroy(go);
        GetComponent<moreSpecificProfile>().publicSetIsDefending(false);
        GetComponent<moreSpecificProfile>().turnToBoss(boss);
    }

    public IEnumerator cooldownSpellDefense()
    {
        yield return new WaitForSeconds(timeCoolDownDefenseSpell);
        cooldownDefense = false;
        GetComponent<moreSpecificProfile>().setDefUsed(false);
    }

    public void activateUlti()
    {
        GetComponent<moreSpecificProfile>().turnToBoss(boss);
        cooldownSpecial = true;

        StartCoroutine(chargingSpecialDuration());
        StartCoroutine(waitAfterUlti());
    }

    public IEnumerator chargingSpecialDuration()
    {
        go = Instantiate(fireBall, pointSpawnUlt.position, transform.rotation, gameObject.transform);
        chargingUlt = true;
        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(specialDuration);
        chargingUlt = false;

        boss.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(200.0f);
        Destroy(go);
        //Debug.Log("ULTI FINITA");
    }

    public IEnumerator waitAfterUlti()
    {
        //Debug.Log("ULTI IN COOLDOWN");
        yield return new WaitForSeconds(timeCoolDownSpecial);
        cooldownSpecial = false;
        //Debug.Log("ULTI UP");
    }

    public void rollAway()
    {
        cooldownDash = true;

        correctRoll();

        StartCoroutine(endDash());
        StartCoroutine(waitRollCollDown());
    }

    public IEnumerator endDash()
    {
        yield return new WaitForSeconds(1.0f);
        isDashing = false;
    }

    public IEnumerator waitRollCollDown()
    {
        yield return new WaitForSeconds(timeRollCooldown);
        cooldownDash = false;
        rb.velocity = Vector3.zero;
    }


    public void correctRoll()
    {
        GetComponent<moreSpecificProfile>().rollAwayChamp(rb, m_MaxDistance, m_PlayerMask, dashForce);
    }


    public void setBoss(GameObject bo)
    {
        boss = bo;
    }

}
