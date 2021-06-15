using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProfile : moreSpecificProfile
{

    private bool cooldownSword = false;
    public bool cooldownShield = false;
    public bool cooldownSpecial = true;
    public bool shieldActive = false;
    public bool swordActive = false;
    public bool ultiRunning = false;
    public bool castingUlt = false;

    public bool cooldownDash = false;
    public bool isDashing = false;

    public GameObject sword;
    public GameObject shieldTank;

    private GameObject boss;
    public Rigidbody rb;
    private GameObject go;
    private Transform pointSpawnSword;
    private Transform pointSpawnShield;
    private Transform pointSpawnHealHealer;

    
    private float swordDuration = 1.0f;
    private float timeCoolDownSwordAttack = 2.0f;
    private float shieldDuration = 2.0f;
    private float timeCoolDownShield = 12.0f;
    private float specialDuration = 6.0f;
    public float timeCoolDownSpecial = 22.0f;

    private float timeRollCooldown = 2.0f;

    private float dashForce = 17.0f;

    private float previousUltiHp = 0.0f;
    private float previousUltiArmor = 0.0f;



    float m_MaxDistance = 10.0f;
    public LayerMask m_PlayerMask;

    // Start is called before the first frame update
    void Start()
    {

       // rb = GetComponent<Rigidbody>();
        pointSpawnSword = transform.GetChild(1);
        pointSpawnShield = transform.GetChild(2);
        pointSpawnHealHealer = transform.GetChild(3);

        StartCoroutine(waitAfterUlti());
    }




    public void attackWithSword()
    {
        //Debug.Log("attackWithSword");
        if (!cooldownSword)
        {
            GetComponent<moreSpecificProfile>().turnToBoss(boss);
            go = Instantiate(sword, pointSpawnSword.position, transform.rotation, gameObject.transform);
            cooldownSword = true;
            swordActive = true;
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



    public void defendWithShield()
    {
        GetComponent<moreSpecificProfile>().publicSetIsDefending(true);
        GetComponent<moreSpecificProfile>().setDefUsed(true);
        GetComponent<moreSpecificProfile>().turnToBoss(boss);

        go = Instantiate(shieldTank, pointSpawnShield.position, transform.rotation, gameObject.transform);
        cooldownShield = true;
        shieldActive = true;
        StartCoroutine(waitBeforeRemoveShield());
        StartCoroutine(cooldownDefense());
    }

    public IEnumerator waitBeforeRemoveShield()
    {
        yield return new WaitForSeconds(shieldDuration);
        shieldActive = false;
        Destroy(go);

        GetComponent<moreSpecificProfile>().publicSetIsDefending(false);
    }

    public IEnumerator cooldownDefense()
    {
        yield return new WaitForSeconds(timeCoolDownShield);
        cooldownShield = false;
        GetComponent<moreSpecificProfile>().setDefUsed(false);
    }

    public void specialInAction()
    {
        castingUlt = true;
        go = Instantiate(sword, pointSpawnHealHealer.position, transform.rotation, gameObject.transform);
        go.transform.rotation = Quaternion.Euler(new Vector3(0f, 90.0f, 0f));
        StartCoroutine(castSpecial());
        
    }

    public IEnumerator castSpecial()
    {
        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(0.8f);
        Destroy(go);
        transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        castingUlt = false;
        ultiRunning = true;
        cooldownSpecial = true;

        previousUltiHp = this.GetComponent<moreSpecificProfile>().publicGetCurrentLife();
        previousUltiArmor = this.GetComponent<moreSpecificProfile>().getArmor();
        Debug.Log("ULTIIIII TANKKK PREVIOUS HP E ARMOR " + previousUltiHp + "   " + previousUltiArmor);
        this.GetComponent<moreSpecificProfile>().incrementStatsUltiTank();

        StartCoroutine(waitBeforeStopSpecial());

        StartCoroutine(waitAfterUlti());
        //Debug.Log("ULTI FINITA");
    }

    public IEnumerator waitBeforeStopSpecial()
    {
        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(specialDuration);

        transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        Debug.Log("ULTIIIII TANKKK PREVIOUS HP E ARMOR " + previousUltiHp + "   " + previousUltiArmor);
        this.GetComponent<moreSpecificProfile>().resetStatsTankUlti(previousUltiHp, previousUltiArmor);

        previousUltiHp = 0.0f;
        previousUltiArmor = 0.0f;

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
        isDashing = true; ;

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
