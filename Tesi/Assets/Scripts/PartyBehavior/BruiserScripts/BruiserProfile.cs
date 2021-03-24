using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;
    private bool cooldownSword = false;
    public bool cooldownHeal = false;
    public bool cooldownSpecial = true;
    public bool swordActive = false;

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
        yield return new WaitForSeconds(1.0f);
        swordActive = false;
        Destroy(go);
    }

    public IEnumerator cooldownAttack()
    {
        yield return new WaitForSeconds(2.8f);
        cooldownSword = false;
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
