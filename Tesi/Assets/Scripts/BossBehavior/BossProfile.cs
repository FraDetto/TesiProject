using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;

    public Collider collider;
    public bool isAttacking;
    public string target;
    public PartyData partyData;
    // Start is called before the first frame update

    void Start()
    {
        hp = partyData.hpBoss;
        damage = partyData.damageBoss;
        armor = partyData.armorBoss;
    }

    public void takeDamageFromSword(float damageFromCharacter)
    {
        hp -= damageFromCharacter;
        Debug.Log("OH NO MI HAI COLPITO " + hp);

    }

    public void takeDamageFromSpell(float damageFromCharacter)
    {
        hp -= damageFromCharacter;
        Debug.Log("OH NO SPELL MI HAI COLPITO " + hp);
    }
}


