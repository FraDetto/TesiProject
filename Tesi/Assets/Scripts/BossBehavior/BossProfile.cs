using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProfile : moreSpecificProfile
{
 
    public Collider collider;
   
    public string target;
    public bool isAttacking = false;
    public bool isUsingAoE = false;


    private Rigidbody rb;
    private GameObject targetPlayer;
    

    private bool cooldownRangedAttk = false;
    private bool cooldownJumpAttk = false;
    private bool cooldownAoEAttk = false;
    private bool cooldownSwingAttk = false;
    private bool cooldownAheadAttk = false;
    private bool cooldownbreakAttk = false;

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


