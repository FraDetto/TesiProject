using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserProfile : MonoBehaviour
{
    private float totalhp;
    [SerializeField] private float currenthp;
    [SerializeField] private float shield;

    [SerializeField] private float damage;
    [SerializeField] private float armor;
    private bool cooldownSword = false;
    public bool cooldownHeal = false;
    public bool isHealing = false;
    public bool cooldownSpecial = true;
    public bool swordActive = false;
    public bool ultiRunning = false;

    private float timeForSpecial = 16.0f;

    public GameObject sword;
    public GameObject HealSign;

    private GameObject go;
    private Transform pointSpawnSword;
    private Transform HealSignSpawnPoint;
    private Rigidbody myRB;

    public PartyData partyData;

    // Start is called before the first frame update
    void Start()
    {
        totalhp = partyData.hpBruiser;
        currenthp = totalhp;
        damage = partyData.damageBruiser;
        armor = partyData.armorBruiser;


        pointSpawnSword = transform.GetChild(1);
        HealSignSpawnPoint = transform.GetChild(2);

        StartCoroutine(waitAfterUlti());
    }

    public float getDamage()
    {
        return damage;
    }

    public float getTotalHp()
    {
        return totalhp;
    }

    public float getCurrentLife()
    {
        return currenthp;
    }

    public void addLifeByCure(float cure)
    {
        currenthp += cure;
    }

    public bool lifeUnderSixty()
    {
        if(currenthp <= (totalhp/100 * 60))
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
        yield return new WaitForSeconds(1.4f);
        swordActive = false;
        Destroy(go);
    }

    public IEnumerator cooldownAttack()
    {
        yield return new WaitForSeconds(2.5f);
        cooldownSword = false;
    }

    public void calculateDamage(float damage)
    {
        currenthp -= damage;
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
        total = totalhp / 100.0f * 30.0f;
        return total;
    }
    public IEnumerator isHealingHimSelf()
    {
        go = Instantiate(HealSign, HealSignSpawnPoint.position, transform.rotation, gameObject.transform);
        yield return new WaitForSeconds(1.2f);
        currenthp += calculateHeal();
        isHealing = false;
        Destroy(go);
    }

    public IEnumerator cooldownHealing()
    {
        yield return new WaitForSeconds(10.0f);
        cooldownHeal = false;
    }

    public void activateUlti()
    {
        ultiRunning = true;
        cooldownSpecial = true;

        StartCoroutine(specialDuration());

        StartCoroutine(waitAfterUlti());
    }

    public IEnumerator specialDuration()
    {
        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(6.2f);
        ultiRunning = false;
        
        //Debug.Log("ULTI FINITA");
    }

    public IEnumerator waitAfterUlti()
    {
        //Debug.Log("ULTI IN COOLDOWN");
        yield return new WaitForSeconds(timeForSpecial);
        cooldownSpecial = false;
        //Debug.Log("ULTI UP");
    }
}
