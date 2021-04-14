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
    public bool ultiRunning = false;


    public GameObject sword;
    public GameObject HealSign;

    private GameObject go;
    private Transform pointSpawnSword;
    private Transform HealSignSpawnPoint;
    private Rigidbody myRB;

    private float swordDuration = 1.4f;
    private float timeCoolDownSwordAttack = 2.5f;
    private float healingHimselfDuration = 1.2f;
    private float timeCoolDownheal = 10.0f;
    private float specialDuration = 6.2f;
    private float timeCoolDownSpecial = 16.0f;

    // Start is called before the first frame update
    void Start()
    {

        pointSpawnSword = transform.GetChild(1);
        HealSignSpawnPoint = transform.GetChild(2);

        StartCoroutine(waitAfterUlti());
    }


    public bool lifeUnderSixty()
    {
        if(getCurrentLife() <= (getTotalLife()/100 * 60))
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

    public void calculateDamage(float damage)
    {
        setLifeAfterDamage(damage);
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
        total = getTotalLife() / 100.0f * 30.0f;
        return total;
    }
    public IEnumerator isHealingHimSelf()
    {
        go = Instantiate(HealSign, HealSignSpawnPoint.position, transform.rotation, gameObject.transform);
        yield return new WaitForSeconds(healingHimselfDuration);
        addLifeByCure(calculateHeal());
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
        ultiRunning = true;
        cooldownSpecial = true;

        StartCoroutine(waitBeforeStopSpecial());

        StartCoroutine(waitAfterUlti());
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
}
