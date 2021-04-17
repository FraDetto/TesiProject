using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProfile : moreSpecificProfile
{
   
    
    public bool isAttacking = false;
    public bool isUsingAoE = false;
    public int[] playersParty;
    public float speedRangedAttk = 25.0f;

    public int codeAttack = 0; // 0:swing  1: ahead  2:break    i use it to divide the different attack and apply the correct damage

    public GameObject goRangedAttk;
    public GameObject swordSwingAttk;
    public GameObject swordAheadAttk;
    public GameObject containerAoEAttk;

    private Rigidbody rb;
    public GameObject targetPlayer;
    public string target;
    private GameObject go;

    private Transform rangedAttackPosition;
    private Transform swingAttackPosition;
    private Transform aheadAttackPosition;//stessa posizione usata per il break
    private Transform AoEAttackPosition;
    private Vector3 scaleChange;

    private float velocityAttraction = 25.0f;

    private bool cooldownRangedAttk = false;
    private bool cooldownRayAttk = false;
    private bool cooldownAoEAttk = false;
    private bool cooldownSwingAttk = false;
    private bool cooldownAheadAttk = false;
    private bool cooldownBreakAttk = false;

    public bool isShooting = false;
    public bool isAttracting = false;
    public bool isCastingAoE = false;


    
    private float timeCoolDownRangedAttack = 6.4f;
    private float timeCoolDownRayAttack = 10.4f;
    private float attractingRootDuration = 2.0f;
    private float AoEDuration = 1.8f;
    private float timeCoolDownAoEAttack = 8.4f;
    private float timeCoolDownSwingAttack= 1.6f;
    private float timeCoolDownAheadAttack = 1.6f;
    private float timeCoolDownBreakAttack = 4.4f;



    public void Start()
    {
        playersParty = new int[4];
        rangedAttackPosition = transform.GetChild(1);
        swingAttackPosition = transform.GetChild(2);
        aheadAttackPosition = transform.GetChild(3);
        AoEAttackPosition = transform.GetChild(4);
        //assign i player all'array

        rb = GetComponent<Rigidbody>();

        scaleChange = new Vector3(0.18f, 0.02f, 0.18f);

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

            if (targetPlayer.tag.Equals("Tank"))
            {

                if (targetPlayer.transform.GetComponent<MageProfile>().defenseActive)
                {
                    isAttracting = false;
                    //stessa cosa di prima per far vedere che fallisce
                    Debug.Log("FALLITO RAY STA DIFENDENDO");
                }
            }
            else if (targetPlayer.tag.Equals("Mage"))
            {

                if (targetPlayer.transform.GetComponent<MageProfile>().defenseActive)
                {
                    isAttracting = false;
                    //stessa cosa di prima per far vedere che fallisce
                    Debug.Log("FALLITO RAY STA DIFENDENDO");
                }

            }

        }


        if (isCastingAoE)
        {
            go.transform.localScale += scaleChange * Time.deltaTime;
        }


    }      


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            rangedAttack();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            target = targetPlayer.tag;
            LaunchRay();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            swingAttack();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            aheadAttack();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            breakAttack();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            AoEAttack();
        }
    }


    public void takeDamageFromSword(float damageFromCharacter)
    {
        publicSetLifeAfterDamage(damageFromCharacter);       
        //Debug.Log("OH NO MI HAI COLPITO " + publicGetCurrentLife());
    }

    public void takeDamageFromSpell(float damageFromCharacter)
    {
        publicSetLifeAfterDamage(damageFromCharacter);
        //Debug.Log("OH NO SPELL MI HAI COLPITO " + publicGetCurrentLife());
    }



    public void rangedAttack()
    {
        //target gia' scelto? o lo sceglie qua?se facessi due brain una per target una per azioni vere e do' reward combinato?
        turnBossToTarget();
        go = Instantiate(goRangedAttk, rangedAttackPosition.position, transform.rotation, gameObject.transform);
        isShooting = true;
        cooldownRangedAttk = true;
        StartCoroutine(startCooldownRangedAttk());//gestire distruzione proiettile
    }

    public IEnumerator startCooldownRangedAttk()
    {
        //ricordarsi di gestire i cooldown
        yield return new WaitForSeconds(timeCoolDownRangedAttack);
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
        turnBossToTarget();
        //  CONTROLLLO PER DIFESA MAGE E TANK: SE SCUDO O SPELL ATTIVA o DISPONIBILE in teoria ray non va a segno

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
            Debug.Log("FALLITO RAY STA DIFENDENDO");
        }
        /*
        if (targetPlayer.tag.Equals("Tank"))
        {
            while (isAttracting)
            {
                if (targetPlayer.transform.GetComponent<MageProfile>().defenseActive)
                {
                    isAttracting = false;
                    //stessa cosa di prima per far vedere che fallisce
                    Debug.Log("FALLITO RAY STA DIFENDENDO");
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
                    Debug.Log("FALLITO RAY STA DIFENDENDO");
                }
            }
        }*/
           

    }



    public IEnumerator startCooldownRayAttk()
    {
        yield return new WaitForSeconds(timeCoolDownRayAttack);
        cooldownRayAttk = false;
    }

    public void AoEAttack()
    {
        go = Instantiate(containerAoEAttk, AoEAttackPosition.position, transform.rotation, gameObject.transform);
        isCastingAoE = true;
        cooldownAoEAttk = true;
        StartCoroutine(castingAoEAttack());
        StartCoroutine(startCooldownAoEAttk());
    }

    public IEnumerator castingAoEAttack()
    {
        yield return new WaitForSeconds(AoEDuration);
        isCastingAoE = false;
        Destroy(go);
    }

    public IEnumerator startCooldownAoEAttk()
    {
        yield return new WaitForSeconds(timeCoolDownAoEAttack);
        cooldownAoEAttk = false;
    }

    public void swingAttack()
    {
        turnBossToTarget();
        go = Instantiate(swordSwingAttk, swingAttackPosition.position, transform.rotation, gameObject.transform);
        cooldownSwingAttk = true;
        codeAttack = 0;
        StartCoroutine(waitBeforeRemoveSword());
        StartCoroutine(startCooldownSwingAttk());
    }

    public IEnumerator startCooldownSwingAttk()
    {
        yield return new WaitForSeconds(timeCoolDownSwingAttack);
        cooldownSwingAttk = false;
    }

    public void aheadAttack()
    {
        turnBossToTarget();
        go = Instantiate(swordAheadAttk, aheadAttackPosition.position, transform.rotation, gameObject.transform);
        cooldownAheadAttk = true;
        codeAttack = 1;
        StartCoroutine(waitBeforeRemoveSword());
        StartCoroutine(startCooldownAheadAttk());
    }

    public IEnumerator startCooldownAheadAttk()
    {
        yield return new WaitForSeconds(timeCoolDownAheadAttack);
        cooldownAheadAttk = false;
    }

    public IEnumerator waitBeforeRemoveSword()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(go);
    }

    public void breakAttack()//wounds that limits healing and reduce armor for tot sec.
    {
        turnBossToTarget();
        cooldownBreakAttk = true;
        go = Instantiate(swordAheadAttk, aheadAttackPosition.position, transform.rotation, gameObject.transform);//per il momento uguale a aheadAttack poi va cambiato
        targetPlayer.GetComponent<moreSpecificProfile>().applyWound();//applica ferite gravi
        targetPlayer.GetComponent<moreSpecificProfile>().reduceArmor();//applica riduzione armor
        codeAttack = 2;
        //danno basso
        StartCoroutine(waitBeforeRemoveSword());
        StartCoroutine(startCooldownBreakAttk());
    }

    public IEnumerator startCooldownBreakAttk()
    {
        yield return new WaitForSeconds(timeCoolDownBreakAttack);
        cooldownBreakAttk = false;
    }


    public void turnBossToTarget()
    {
        Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, transform.position.y, targetPlayer.transform.position.z);
        transform.LookAt(verticalAdj);
    }

}


