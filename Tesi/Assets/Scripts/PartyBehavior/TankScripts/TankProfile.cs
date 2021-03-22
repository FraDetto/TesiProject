using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;
    public GameObject sword;
    public GameObject shield;


    private Rigidbody myRB;
    private Transform pointSpawnSword;

    public PartyData partyData;

    // Start is called before the first frame update
    void Start()
    { 
        hp = partyData.hpTank;
        damage = partyData.damageTank;
        armor = partyData.armorTank;

        pointSpawnSword = transform.GetChild(1);

    }

    public float getDamage() {
        return damage;
    }
    
    public void attackWithSword()
    {
        //Debug.Log("attackWithSword");
       
        StartCoroutine(waitAfterAttack());
    }

    public IEnumerator waitAfterAttack()
    {
        GameObject go = Instantiate(sword, pointSpawnSword.position, transform.rotation, gameObject.transform);
        yield return new WaitForSeconds(1.0f);
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
