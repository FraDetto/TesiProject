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

    private float velocityAttraction = 25.0f;

    private bool cooldownRangedAttk = false;
    private bool cooldownRayAttk = false;
    private bool cooldownAoEAttk = false;
    private bool cooldownSwingAttk = false;
    private bool cooldownAheadAttk = false;
    private bool cooldownBreakAttk = false;

    public bool isShooting = false;
    public bool isAttracting = false;


    private Vector3 initialPosition;
    private float attractingRootDuration = 2.0f;


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


        if (isAttracting)
        {
            Vector3 verticalAdjBoss = new Vector3(targetPlayer.transform.position.x, transform.position.y, targetPlayer.transform.position.z);
            Vector3 verticalAdj = new Vector3(transform.position.x, targetPlayer.transform.position.y, transform.position.z);
            Vector3 toBossPos = (verticalAdj - targetPlayer.transform.position);

            if ((transform.position - targetPlayer.GetComponent<Rigidbody>().transform.position).magnitude > 8.0f)
            {
                transform.LookAt(verticalAdjBoss);
                targetPlayer.transform.LookAt(verticalAdj);
                targetPlayer.GetComponent<Rigidbody>().MovePosition(targetPlayer.transform.position + (targetPlayer.transform.forward) * velocityAttraction * Time.deltaTime);
            }
            else
            {
                isAttracting = false;
                targetPlayer.GetComponent<moreSpecificProfile>().publicAddRootStatus(attractingRootDuration);//root player
                rayAttack();
            }
               
        }

  
    }
        


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchRay();
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

    public void rayAttack()
    {
        cooldownRayAttk = true;

        StartCoroutine(startCooldownRayAttk());
    }



    public void LaunchRay()
    {
        Debug.Log("HO ATTIVATO RAY");
        // AGGIUGNERE CONTROLLLO PER DIFESA MAGE E TANK: SE SCUDO O SPELL ATTIVA o DISPONIBILE in teoria ray non va a segno

        bool enemyIsDefending;

        switch (targetPlayer.tag)
        {
            case "Tank":
                enemyIsDefending = targetPlayer.transform.GetComponent<TankProfile>().shieldActive;

                break;

            case "Mage":
                enemyIsDefending = targetPlayer.transform.GetComponent<MageProfile>().defenseActive;
                break;

            default:
                enemyIsDefending = false;
                break;
        }
        if (!enemyIsDefending)
        {
            isAttracting = true;
        }
        else
        {
            //fai qualcosa epr far vedere che e' fallito il ray ma se sta difendendo bad reward
        }

        if (targetPlayer.tag.Equals("Tank"))
        {
            while (isAttracting)
            {
                if (targetPlayer.transform.GetComponent<MageProfile>().defenseActive)
                {
                    isAttracting = false;
                    //stessa cosa di prima per far vedere che fallisce
                }
            }
        }else if (targetPlayer.tag.Equals("Mage"))
        {
            while (isAttracting)
            {
                if (targetPlayer.transform.GetComponent<MageProfile>().defenseActive)
                {
                    isAttracting = false;
                    //stessa cosa di prima per far vedere che fallisce
                }
            }
        }
           

    }



    public IEnumerator startCooldownRayAttk()
    {
        yield return new WaitForSeconds(10.4f);
        cooldownRayAttk = false;
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


