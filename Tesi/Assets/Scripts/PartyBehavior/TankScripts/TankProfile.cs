using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;
    private bool cooldown = false;


    public GameObject sword;
    public GameObject shield;


    private Rigidbody myRB;
    private GameObject go;
    private Transform pointSpawnSword;
    private Transform pointSpawnShield;

    public PartyData partyData;

    // Start is called before the first frame update
    void Start()
    { 
        hp = partyData.hpTank;
        damage = partyData.damageTank;
        armor = partyData.armorTank;

        pointSpawnSword = transform.GetChild(1);
        pointSpawnShield = transform.GetChild(2);

    }

    public float getDamage() {
        return damage;
    }
    
    public void attackWithSword()
    {
        //Debug.Log("attackWithSword");
        if (!cooldown)
        {
            go = Instantiate(sword, pointSpawnSword.position, transform.rotation, gameObject.transform);
            cooldown = true;
            StartCoroutine(waitAfterAttack());
        }
        
    }

    public IEnumerator waitAfterAttack()
    {
        yield return new WaitForSeconds(1.0f);
        cooldown = false;
        Destroy(go);
    }
    public void calculateDamage()
    {

    }

    public void defendWithShield()
    {

    }

    public void activateUlti()
    {

    }

}
