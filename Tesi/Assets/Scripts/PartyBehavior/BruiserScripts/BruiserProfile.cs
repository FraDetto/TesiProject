using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;
    private bool cooldown = false;


    public GameObject sword;

    private GameObject go;
    private Transform pointSpawnSword;
    private Rigidbody myRB;

    public PartyData partyData;

    // Start is called before the first frame update
    void Start()
    {
        hp = partyData.hpBruiser;
        damage = partyData.damageBruiser;
        armor = partyData.armorBruiser;

        pointSpawnSword = transform.GetChild(1);
    }

    public float getDamage()
    {
        return damage;
    }

    public void attackWithSword()
    {
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

    public void healHimSelf()
    {

    }

    public void activateUlti()
    {

    }
}
