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

    public PartyData partyData;

    // Start is called before the first frame update
    void Start()
    { 
        hp = partyData.hpTank;
        damage = partyData.damageTank;
        armor = partyData.armorTank;

    }

    public float getDamage() {
        return damage;
    }
    
    public void attackWithSword()
    {
        Debug.Log("attackWithSword");
       
        StartCoroutine(waitAfterAttack());
    }

    public IEnumerator waitAfterAttack()
    {
        GameObject go = Instantiate(sword, new Vector3(transform.position.x+2.9f , transform.position.y + 0.8f, transform.position.z -0.45f), transform.rotation, gameObject.transform);
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
