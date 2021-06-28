using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerProfile : moreSpecificProfile
{

    //public bool shooting = false;
    private bool cooldownAttack = false;

    public bool cooldownHeal = false;
    public bool cooldownSpecial = true;
    public bool healActive = false;
    public bool chargingUlt = false;

    public bool cooldownDash = false;
    public bool isDashing = false;

    private GameObject boss;
    //private GameObject tank;
    //private GameObject bruiser;
    //private GameObject mage;

    public Transform pointSpawnWindBall;
    public Transform pointSpawnHealHealer;
    public Rigidbody rb;
    private GameObject go;
    private GameObject hs;

    public GameObject healingText;
    public GameObject windBall;


    public float m_HealRadius = 80f;
    public LayerMask m_PlayerMask;

    public float speed = 25.0f;


    private float timeCoolDownMagicAttack = 2.0f;
    private float healingActivation = 1.5f;
    private float timeCoolDownheal = 8.0f;
    private float timeSpecialActivation = 0.8f;
    private float timeCoolDownSpecial = 20.0f;

    private float timeRollCooldown = 2.0f;

    private float dashForce = 17.0f;

    float m_MaxDistance = 10.0f;
    public LayerMask m_PlayerMask_Roll;
    // Start is called before the first frame update
    void Start()
    {

        //pointSpawnWindBall = transform.GetChild(1);
        //pointSpawnHealHealer = transform.GetChild(2);

        //boss = GameObject.FindGameObjectWithTag("Boss");

        /*tank = GameObject.FindGameObjectWithTag("Tank");
        mage = GameObject.FindGameObjectWithTag("Mage");
        bruiser = GameObject.FindGameObjectWithTag("Bruiser");*/

        //rb = GetComponent<Rigidbody>();

        StartCoroutine(waitAfterUlti());
    }

    private void FixedUpdate()
    {
        if(GetComponent<moreSpecificProfile>().publicGetStatus() != 0)
        {
            if (boss.GetComponentInChildren<BossMovingBehavior>().bossIsRunning)
            {
                GetComponent<moreSpecificProfile>().publicSetPreviousStatus(0);
            }
        }

        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            if (GetComponent<moreSpecificProfile>().getShooting() && null!=go)
            {
                go.transform.LookAt(boss.transform.position);
                go.transform.position += go.transform.forward * speed * Time.deltaTime;
            }
        }
        else
        {
            if (null != go)
            {
                Destroy(go.gameObject);
            }
        }
    }
    /*
    private void Update()
    {
        if (!cooldownDash)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                rollAway();
            }
        }

    }*/

    public void attackWithMagic()
    {
        //Debug.Log("attackWithMagic");
        if (!cooldownAttack)
        {
            if (!GetComponent<moreSpecificProfile>().flagResetepisode)
            {
                GetComponent<moreSpecificProfile>().turnToBoss(boss);
                go = Instantiate(windBall, pointSpawnWindBall.position, transform.rotation, gameObject.transform);
                //shooting = true;
                GetComponent<moreSpecificProfile>().setShooting(true);
                cooldownAttack = true;
                StartCoroutine(waitAfterAttack());
            }
        }

    }

    public IEnumerator waitAfterAttack()
    {
        yield return new WaitForSeconds(timeCoolDownMagicAttack);
        cooldownAttack = false;
    }


    public void healAlly()
    {
        //Debug.Log("SONO IN HEALALLY" );
        healActive = true;
        cooldownHeal = true;
        findAllyToHeal();
        StartCoroutine(waitCastHeal());
        StartCoroutine(cooldownHealing());
    }

    public void findAllyToHeal()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_HealRadius, m_PlayerMask);
        int index = 0;
        float minLife = 0.0f;

        for (int i = 0; i < colliders.Length; i++)
        {
            moreSpecificProfile targetProfile = colliders[i].GetComponent<moreSpecificProfile>();

            //Debug.Log("SONO IN FINDHALLY: " + targetProfile.tag);
            if (!targetProfile)
                continue;
            if (targetProfile.getStatusLifeChamp()==0)
            {
                if (i == 0)
                {
                    index = i;
                    minLife = targetProfile.publicGetCurrentLife() / targetProfile.publicGetTotalLife() * 100;
                }
                else
                {
                    if (minLife > (targetProfile.publicGetCurrentLife() / targetProfile.publicGetTotalLife() * 100))
                    {
                        minLife = (targetProfile.publicGetCurrentLife() / targetProfile.publicGetTotalLife() * 100);
                        index = i;
                    }
                }
            }
           
        }
        float cure = colliders[index].GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40;
        colliders[index].GetComponent<moreSpecificProfile>().publicAddLifeByCure(cure);
        hs = Instantiate(healingText, colliders[index].transform.Find("pointSpawnHealHealer").position, colliders[index].transform.rotation, colliders[index].transform);
        
        //Debug.Log("STO HEALANDO: " + colliders[index].tag);
    }



    public IEnumerator waitCastHeal()
    {
        yield return new WaitForSeconds(healingActivation);
        healActive = false;
        Destroy(hs);
    }

    public IEnumerator cooldownHealing()
    {
        yield return new WaitForSeconds(timeCoolDownheal);
        cooldownHeal = false;
    }


    public void activateUlti()
    {
        cooldownSpecial = true;

        StartCoroutine(specialActivation());
        StartCoroutine(waitAfterUlti());
    }

    public IEnumerator specialActivation()
    {
      
        chargingUlt = true;

        go = Instantiate(windBall, pointSpawnHealHealer.position, transform.rotation, gameObject.transform);

        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(timeSpecialActivation);

        //Shield applied after time of activation
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_HealRadius, m_PlayerMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            moreSpecificProfile targetProfile = colliders[i].GetComponent<moreSpecificProfile>();

            if (!targetProfile)
                continue;
            if (targetProfile.getStatusLifeChamp() == 0)
            {
                float shieldToApply = (targetProfile.publicGetTotalLife() / 100) * 40;
                targetProfile.publicAddShield(shieldToApply);
            }

        }

        chargingUlt = false;
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
        GetComponent<moreSpecificProfile>().rollAwayChamp(rb, m_MaxDistance, m_PlayerMask_Roll, dashForce);
    }

    public void setBoss(GameObject bo)
    {
        boss = bo;
    }

}
