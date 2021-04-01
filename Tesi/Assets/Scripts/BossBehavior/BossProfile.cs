using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProfile : moreSpecificProfile
{
 
    public Collider collider;
    public bool isAttacking;
    public bool isUsingAoE;
    public string target;
  
    // Start is called before the first frame update


    public void takeDamageFromSword(float damageFromCharacter)
    {
        publicSetLifeAfterDamage(damageFromCharacter);
        
        Debug.Log("OH NO MI HAI COLPITO " + publicGetCurrentLife());

    }

    public void takeDamageFromSpell(float damageFromCharacter)
    {
        publicSetLifeAfterDamage(damageFromCharacter);
        Debug.Log("OH NO SPELL MI HAI COLPITO " + publicGetCurrentLife());
    }
}


