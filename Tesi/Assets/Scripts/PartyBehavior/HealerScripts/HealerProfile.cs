using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerProfile : moreSpecificProfile
{

    public bool shooting = false;
    private bool cooldownAttack = false;

    public bool cooldownHeal = false;
    public bool cooldownSpecial = true;
    public bool healActive = false;
    public bool chargingUlt = false;


    private GameObject boss;

    private GameObject tank;
    private GameObject bruiser;
    private GameObject mage;

    private Transform pointSpawnWindBall;
    private Transform pointSpawnHealHealer;
    private Rigidbody myRB;
    private GameObject go;

    public GameObject windBall;


    public float m_HealRadius = 100f;
    public LayerMask m_PlayerMask;


    private float timeForSpecial = 16.0f;
    public float speed = 25.0f;

    // Start is called before the first frame update
    void Start()
    {

        pointSpawnWindBall = transform.GetChild(1);
        pointSpawnHealHealer = transform.GetChild(2);

        boss = GameObject.FindGameObjectWithTag("Boss");

        tank = GameObject.FindGameObjectWithTag("Tank");
        mage = GameObject.FindGameObjectWithTag("Mage");
        bruiser = GameObject.FindGameObjectWithTag("Bruiser");


        StartCoroutine(waitAfterUlti());
    }

    private void FixedUpdate()
    {
        if (shooting)
        {
            go.transform.LookAt(boss.transform.position);
            go.transform.position += go.transform.forward * speed * Time.deltaTime;
        }
    }

    public void attackWithMagic()
    {
        //Debug.Log("attackWithMagic");
        if (!cooldownAttack)
        {
            go = Instantiate(windBall, pointSpawnWindBall.position, transform.rotation, gameObject.transform);
            shooting = true;
            cooldownAttack = true;
            StartCoroutine(waitAfterAttack());
        }

    }

    public IEnumerator waitAfterAttack()
    {

        yield return new WaitForSeconds(2.0f);
        cooldownAttack = false;
    }

    public void calculateDamage()
    {

    }

    public void healAlly()
    {
        Debug.Log("SONO IN HEALALLY" );
        healActive = true;
        cooldownHeal = true;
        findAllyToHeal();
        StartCoroutine(waitCastHeal());
        StartCoroutine(cooldownDefense());
    }

    public void findAllyToHeal()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_HealRadius, m_PlayerMask);
        int index = 0;
        float minLife = 0.0f;

        for (int i = 0; i < colliders.Length; i++)
        {
            moreSpecificProfile targetProfile = colliders[i].GetComponent<moreSpecificProfile>();

            Debug.Log("SONO IN FINDHALLY: " + targetProfile.tag);
            if (!targetProfile)
                continue;
            if (i == 0)
            {
                index = i;
                minLife = targetProfile.publicGetCurrentLife() / targetProfile.publicGetTotalLife() * 100;

            }
            else
            {
                if(minLife > (targetProfile.publicGetCurrentLife() / targetProfile.publicGetTotalLife() * 100))
                {
                    minLife = (targetProfile.publicGetCurrentLife() / targetProfile.publicGetTotalLife() * 100);
                    index = i;
                }
            }
            
        }
        float cure = colliders[index].GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40;
        colliders[index].GetComponent<moreSpecificProfile>().publicAddLifeByCure(cure);
        Debug.Log("STO HEALANDO: " + colliders[index].tag);
    }



    public IEnumerator waitCastHeal()
    {
        yield return new WaitForSeconds(2.0f);
        healActive = false;
        Destroy(go);
    }

    public IEnumerator cooldownDefense()
    {
        yield return new WaitForSeconds(10.0f);
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

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_HealRadius, m_PlayerMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            moreSpecificProfile targetProfile = colliders[i].GetComponent<moreSpecificProfile>();

            if (!targetProfile)
                continue;
            targetProfile.publicAddShield(60.0f);

        }

        Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(1.5f);
        chargingUlt = false;
        Destroy(go);
        
        Debug.Log("ULTI FINITA");
    }

  

    public IEnumerator waitAfterUlti()
    {
        //Debug.Log("ULTI IN COOLDOWN");
        yield return new WaitForSeconds(timeForSpecial);
        cooldownSpecial = false;
        //Debug.Log("ULTI UP");
    }
}
