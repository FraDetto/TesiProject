using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProfile : moreSpecificProfile
{
   
    public string target;
    public bool isAttacking = false;
    public bool isUsingAoE = false;
    public int[] playersParty;

    private Rigidbody rb;
    private GameObject targetPlayer;

    private Transform rangedAttackPosition;
    private Transform swingAttackPosition;
    private Transform aheadAttackPosition;
    private Transform breakAttackPosition;
   

    private bool cooldownRangedAttk = false;
    private bool cooldownJumpAttk = false;
    private bool cooldownAoEAttk = false;
    private bool cooldownSwingAttk = false;
    private bool cooldownAheadAttk = false;
    private bool cooldownBreakAttk = false;

    // Start is called before the first frame update

    public void Start()
    {
        playersParty = new int[4];
    }

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



    public void rangedAttack()
    {
        cooldownRangedAttk = true;
        StartCoroutine(startCooldownRangedAttk());
    }

    public IEnumerator startCooldownRangedAttk()
    {
        yield return new WaitForSeconds(1.4f);
        cooldownRangedAttk = false;
    }

    public void jumpAttack()
    {
        cooldownJumpAttk = true;
        StartCoroutine(startCooldownJumpAttk());
    }

    public IEnumerator startCooldownJumpAttk()
    {
        yield return new WaitForSeconds(1.4f);
        cooldownJumpAttk = false;
    }

    public void AoEAttack()
    {
        cooldownAoEAttk = true;
        StartCoroutine(startCooldownAoEAttk());
    }

    public IEnumerator startCooldownAoEAttk()
    {
        yield return new WaitForSeconds(1.4f);
        cooldownAoEAttk = false;
    }

    public void SwingAttack()
    {
        cooldownSwingAttk = true;
        StartCoroutine(startCooldownSwingAttk());
    }

    public IEnumerator startCooldownSwingAttk()
    {
        yield return new WaitForSeconds(1.4f);
        cooldownSwingAttk = false;
    }

    public void aheadAttack()
    {
        cooldownAheadAttk = true;
        StartCoroutine(startCooldownAheadAttk());
    }

    public IEnumerator startCooldownAheadAttk()
    {
        yield return new WaitForSeconds(1.4f);
        cooldownAheadAttk = false;
    }

    public void breakAttack()
    {
        cooldownBreakAttk = true;
        StartCoroutine(startCooldownBreakAttk());
    }

    public IEnumerator startCooldownBreakAttk()
    {
        yield return new WaitForSeconds(1.4f);
        cooldownBreakAttk = false;
    }
}


