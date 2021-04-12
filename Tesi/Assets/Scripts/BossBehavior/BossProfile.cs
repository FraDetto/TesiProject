using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProfile : moreSpecificProfile
{
   
    public string target;
    public bool isAttacking = false;
    public bool isUsingAoE = false;
    public int[] playersParty;
    public float speedRangedAttk = 25.0f;

    public GameObject goRangedAttk;
    public GameObject swordSwingAttk;
    public GameObject swordAheadAttk;

    private Rigidbody rb;
    public GameObject targetPlayer;
    private GameObject go;

    private Transform rangedAttackPosition;
    private Transform swingAttackPosition;
    private Transform aheadAttackPosition;//stessa posizione usata per il break
   

    private bool cooldownRangedAttk = false;
    private bool cooldownJumpAttk = false;
    private bool cooldownAoEAttk = false;
    private bool cooldownSwingAttk = false;
    private bool cooldownAheadAttk = false;
    private bool cooldownBreakAttk = false;

    public bool isShooting = false;


    public float firingAngle = 45.0f;
    public float gravity = 5.0f;

    private bool bTargetReady;

    private Vector3 initialPosition;
  

    // Start is called before the first frame update

    public void Start()
    {
        playersParty = new int[4];
        rangedAttackPosition = transform.GetChild(1);
        swingAttackPosition = transform.GetChild(2);
        aheadAttackPosition = transform.GetChild(3);
        //assign i player all'array

        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
    
    }


    private void FixedUpdate()
    {
        if (isShooting)
        {
            go.transform.LookAt(targetPlayer.transform.position);
            go.transform.position += go.transform.forward * speedRangedAttk * Time.deltaTime;
            //bisogna poi cambiare isShooting in false
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }

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
        //target gia' scelto? o lo sceglie qua?se facessi due brain una per target una per azioni vere e do' reward combinato?

        go = Instantiate(goRangedAttk, rangedAttackPosition.position, transform.rotation, gameObject.transform);
        isShooting = true;
        cooldownRangedAttk = true;
        StartCoroutine(startCooldownRangedAttk());//gestire distruzione proiettile
    }

    public IEnumerator startCooldownRangedAttk()
    {
        //ricordarsi di gestire i cooldown
        yield return new WaitForSeconds(6.4f);
        cooldownRangedAttk = false;
    }

    public void jumpAttack()
    {
        cooldownJumpAttk = true;
        bTargetReady = false;
        StartCoroutine(startCooldownJumpAttk());
    }

    // launches the object towards the TargetObject with a given LaunchAngle
    /*void Launch()
    {

        Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 targetXZPos = new Vector3(targetPlayer.transform.position.x, 0.0f, targetPlayer.transform.position.z);

        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(LaunchAngle * Mathf.Deg2Rad);
        float H = targetPlayer.transform.position.y - transform.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rb.velocity = globalVelocity;
        bTargetReady = false;
    }*/
    void Launch()
    {

    }



    public IEnumerator startCooldownJumpAttk()
    {
        yield return new WaitForSeconds(1.4f);
        cooldownJumpAttk = false;
        bTargetReady = true;
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
        go = Instantiate(swordSwingAttk, swingAttackPosition.position, transform.rotation, gameObject.transform);
        cooldownSwingAttk = true;
        StartCoroutine(waitBeforeRemoveSword());
        StartCoroutine(startCooldownSwingAttk());
    }

    public IEnumerator startCooldownSwingAttk()
    {
        yield return new WaitForSeconds(1.6f);
        cooldownSwingAttk = false;
    }

    public void aheadAttack()
    {
        go = Instantiate(swordAheadAttk, swingAttackPosition.position, transform.rotation, gameObject.transform);
        cooldownAheadAttk = true;
        StartCoroutine(waitBeforeRemoveSword());
        StartCoroutine(startCooldownAheadAttk());
    }

    public IEnumerator startCooldownAheadAttk()
    {
        yield return new WaitForSeconds(1.6f);
        cooldownAheadAttk = false;
    }

    public IEnumerator waitBeforeRemoveSword()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(go);
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


