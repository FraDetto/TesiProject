using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;
    private bool cooldownSword = false;
    public bool cooldownShield = false;
    public bool cooldownSpecial = true;


    public GameObject sword;
    public GameObject shield;


    private Rigidbody myRB;
    private GameObject go;
    private Transform pointSpawnSword;
    private Transform pointSpawnShield;

    private float timeForSpecial = 16.0f;

    public PartyData partyData;

    // Start is called before the first frame update
    void Start()
    { 
        hp = partyData.hpTank;
        damage = partyData.damageTank;
        armor = partyData.armorTank;

        pointSpawnSword = transform.GetChild(1);
        pointSpawnShield = transform.GetChild(2);

        StartCoroutine(waitAfterUlti());
    }

    public float getDamage() {
        return damage;
    }
    
    public void attackWithSword()
    {
        //Debug.Log("attackWithSword");
        if (!cooldownSword)
        {
            go = Instantiate(sword, pointSpawnSword.position, transform.rotation, gameObject.transform);
            cooldownSword = true;
            StartCoroutine(waitAfterAttack());
        }
        
    }

    public IEnumerator waitAfterAttack()
    {
        yield return new WaitForSeconds(2.0f);
        cooldownSword = false;
        Destroy(go);
    }

    public void calculateDamage()
    {

    }

    public void defendWithShield()
    {
        go = Instantiate(shield, pointSpawnShield.position, transform.rotation, gameObject.transform);
        cooldownShield = true;
        StartCoroutine(waitBeforeRemoveShield());
        StartCoroutine(cooldownDefense());
    }

    public IEnumerator waitBeforeRemoveShield()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(go);
    }

    public IEnumerator cooldownDefense()
    {
        yield return new WaitForSeconds(10.0f);
        cooldownShield = false;
    }

    public void specialInAction()
    {
        transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        cooldownSpecial = true;
        Debug.Log("APPENA USATA ULTI");
        StartCoroutine(specialDuration());
        
        StartCoroutine(waitAfterUlti());
        
    }

    public IEnumerator specialDuration()
    {
        Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(6.0f);
        transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        Debug.Log("ULTI FINITA");
    }

    public IEnumerator waitAfterUlti()
    {
        Debug.Log("ULTI IN COOLDOWN");
        yield return new WaitForSeconds(timeForSpecial);
        cooldownSpecial = false;
        Debug.Log("ULTI UP");
    }

}
