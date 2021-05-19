using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserProfile : moreSpecificProfile
{
  
    private bool cooldownSword = false;
    public bool cooldownHeal = false;
    public bool isHealing = false;
    public bool cooldownSpecial = true;
    public bool swordActive = false;
    public bool castingUlt = false;
    public bool ultiRunning = false;

    public bool cooldownDash = false;
    public bool isDashing = false;

    public GameObject sword;
    public GameObject HealSign;

    private GameObject boss;
    private GameObject go;
    private Transform pointSpawnSword;
    private Transform HealSignSpawnPoint;
    private Rigidbody rb;

    private float swordDuration = 1.4f;
    private float timeCoolDownSwordAttack = 2.5f;
    private float healingHimselfDuration = 1.2f;
    private float timeCoolDownheal = 10.0f;
    private float specialDuration = 6.2f;
    private float timeCoolDownSpecial = 16.0f;

    private float timeRollCooldown = 2.0f;

    private float dashForce = 17.0f;

    float m_MaxDistance = 10.0f;
    public LayerMask m_PlayerMask;

    // Start is called before the first frame update
    void Start()
    {
        pointSpawnSword = transform.GetChild(1);
        HealSignSpawnPoint = transform.GetChild(2);
        rb = GetComponent<Rigidbody>();

        StartCoroutine(waitAfterUlti());
    }



    public bool lifeUnderSixty()
    {
        if(GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 60))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void attackWithSword()
    {
        if (!cooldownSword)
        {
            GetComponent<moreSpecificProfile>().turnToBoss(boss);
            go = Instantiate(sword, pointSpawnSword.position, transform.rotation, gameObject.transform);
            if (ultiRunning)
            {
                go.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
            }
            cooldownSword = true;
            swordActive = true;

            //calculateDamage(damage);

            StartCoroutine(waitBeforeRemoveSword());
            StartCoroutine(cooldownAttack());
        }
    }
    public IEnumerator waitBeforeRemoveSword()
    {
        yield return new WaitForSeconds(swordDuration);
        swordActive = false;
        Destroy(go);
    }

    public IEnumerator cooldownAttack()
    {
        yield return new WaitForSeconds(timeCoolDownSwordAttack);
        cooldownSword = false;
    }


    public void healHimSelf()
    {
        cooldownHeal = true;
        isHealing = true;
        StartCoroutine(isHealingHimSelf());
        
        StartCoroutine(cooldownHealing());
    }

    private float calculateHeal()
    {
        float total = 0.0f;
        total = (GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100.0f) * 30.0f;
        return total;
    }
    public IEnumerator isHealingHimSelf()
    {
        go = Instantiate(HealSign, HealSignSpawnPoint.position, transform.rotation, gameObject.transform);
        yield return new WaitForSeconds(healingHimselfDuration);
        GetComponent<moreSpecificProfile>().publicAddLifeByCure(calculateHeal());
        isHealing = false;
        Destroy(go);
    }

    public IEnumerator cooldownHealing()
    {
        yield return new WaitForSeconds(timeCoolDownheal);
        cooldownHeal = false;
    }

    public void activateUlti()
    {
        
        castingUlt = true;
        go = Instantiate(sword, HealSignSpawnPoint.position, transform.rotation, gameObject.transform);
        go.transform.rotation = Quaternion.Euler(new Vector3(0f, 90.0f, 0f));
        StartCoroutine(castSpecial());

    }

    public IEnumerator castSpecial()
    {
        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(0.8f);
        Destroy(go);
        castingUlt = false;
        ultiRunning = true;
        cooldownSpecial = true;
        StartCoroutine(waitBeforeStopSpecial());

        StartCoroutine(waitAfterUlti());
        //Debug.Log("ULTI FINITA");
    }

    public IEnumerator waitBeforeStopSpecial()
    {
        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(specialDuration);
        ultiRunning = false;
        
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
