using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageProfile : moreSpecificProfile
{

    public bool shooting = false;
    private bool cooldown = false;

    public bool cooldownDefense = false;
    public bool cooldownSpecial = true;
    public bool defenseActive = false;
    public bool chargingUlt = false;

    public bool cooldownDash = false;
    public bool isDashing = false;

    public bool rightDefSpellDirection = false;

    private GameObject boss;
    private Transform pointSpawnFireBall;
    private Transform pointSpawnUlt;
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
        pointSpawnFireBall = transform.GetChild(1);
        pointSpawnUlt = transform.GetChild(2);

        boss = GameObject.FindGameObjectWithTag("Boss");
        scaleChange = new Vector3(0.1f, 0.1f, 0.1f);

        rb = GetComponent<Rigidbody>();

        StartCoroutine(waitAfterUlti());
    }


    private void FixedUpdate()
    {
        if (shooting)
        {
            go.transform.LookAt(boss.transform.position);
            go.transform.position += go.transform.forward * speedSpells * Time.deltaTime;
        }

        if (chargingUlt)
        {
            go.transform.localScale += scaleChange * Time.deltaTime;
        }
    }



    public void attackWithMagic()
    {
        //Debug.Log("attackWithMagic");
        if (!cooldown)
        {
            
            go = Instantiate(fireBall, pointSpawnFireBall.position, transform.rotation, gameObject.transform);
            shooting = true;
            cooldown = true;
            StartCoroutine(waitAfterAttack());
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
    }

    public IEnumerator cooldownSpellDefense()
    {
        yield return new WaitForSeconds(timeCoolDownDefenseSpell);
        cooldownDefense = false;
    }

    public void activateUlti()
    {
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
}
