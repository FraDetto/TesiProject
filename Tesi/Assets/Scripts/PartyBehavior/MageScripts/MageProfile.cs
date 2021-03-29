using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageProfile : moreSpecificProfile
{
    /*[SerializeField] private float totalhp;
    [SerializeField] private float currenthp;
    [SerializeField] private float shieldValue;

    [SerializeField] private float damage;
    [SerializeField] private float armor;*/

    public bool shooting = false;
    private bool cooldown = false;

    public bool cooldownDefense = false;
    public bool cooldownSpecial = true;
    public bool defenseActive = false;
    public bool chargingUlt = false;

    private GameObject boss;
    private Transform pointSpawnFireBall;
    private Transform pointSpawnUlt;
    private Rigidbody myRB;
    private GameObject go;

    private Vector3 scaleChange;

    public  GameObject fireBall;
    public GameObject defenseSpellSign;

    //protected PartyData partyData;

    public float speedSpells = 25.0f;

    private float timeForSpecial = 16.0f;

    // Start is called before the first frame update
    void Start()
    {
        /*totalhp = partyData.hpMage;

        currenthp = totalhp;

        damage = partyData.damageMage;
        armor = partyData.armorMage;*/


        pointSpawnFireBall = transform.GetChild(1);

        pointSpawnUlt = transform.GetChild(2);

        boss = GameObject.FindGameObjectWithTag("Boss");

        scaleChange = new Vector3(0.1f, 0.1f, 0.1f);

        StartCoroutine(waitAfterUlti());
    }

    public float getDamage()
    {
        return getDamageValue();
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


    public void calculateDamage()
    {

    }

    public void attackWithMagic()
    {
        Debug.Log("attackWithMagic");
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

        yield return new WaitForSeconds(2.0f);
        cooldown = false;
     
    }


    public void defendWithSpell()
    {
        go = Instantiate(defenseSpellSign, pointSpawnUlt.position, transform.rotation, gameObject.transform);
        cooldownDefense = true;
        defenseActive = true;
        StartCoroutine(waitBeforeRemoveShield());
        StartCoroutine(cooldownSpellDefense());
    }

    public IEnumerator waitBeforeRemoveShield()
    {
        yield return new WaitForSeconds(2.0f);
        defenseActive = false;
        Destroy(go);
    }

    public IEnumerator cooldownSpellDefense()
    {
        yield return new WaitForSeconds(10.0f);
        cooldownDefense = false;
    }

    public void activateUlti()
    {
        cooldownSpecial = true;

        StartCoroutine(specialDuration());

        StartCoroutine(waitAfterUlti());
    }

    public IEnumerator specialDuration()
    {
        go = Instantiate(fireBall, pointSpawnUlt.position, transform.rotation, gameObject.transform);
        chargingUlt = true;
        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(3.0f);
        chargingUlt = false;
        Destroy(go);
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
