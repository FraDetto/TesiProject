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

    public bool cooldownDash = false;
    public bool isDashing = false;

    public GameObject sword;
    public GameObject shieldTank;


    private Rigidbody rb;
    private GameObject go;
    private Transform pointSpawnSword;
    private Transform pointSpawnShield;
    private Transform pointSpawnHealHealer;

    
    private float swordDuration = 1.0f;
    private float timeCoolDownSwordAttack = 2.0f;
    private float shieldDuration = 2.0f;
    private float timeCoolDownShield = 10.0f;
    private float specialDuration = 6.0f;
    private float timeCoolDownSpecial = 16.0f;

    private float timeRollCooldown = 2.0f;

    private float dashForce = 17.0f;


    

    float m_MaxDistance = 10.0f;
    public LayerMask m_PlayerMask;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
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
    }

    public void specialInAction()
    {
        transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        cooldownSpecial = true;

        StartCoroutine(waitBeforeStopSpecial());
        
        StartCoroutine(waitAfterUlti());
        
    }

    public IEnumerator waitBeforeStopSpecial()
    {
        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(specialDuration);
        transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
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

    

}
