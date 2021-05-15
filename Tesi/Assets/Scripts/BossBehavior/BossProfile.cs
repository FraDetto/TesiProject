using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProfile : moreSpecificProfile
{
    
    public GameObject[] playersParty;
    public float speedRangedAttk = 30.0f;

    public int codeAttack = 0; // 0:swing  1: ahead  2:break    i use it to divide the different attack and apply the correct damage

    public GameObject goRangedAttk;
    public GameObject swordSwingAttk;
    public GameObject swordAheadAttk;
    public GameObject containerAoEAttk;

    public LayerMask m_PlayerMask;

    private Rigidbody rb;
    public GameObject targetPlayer;
    public int instanceIDtarget;
    //public string target;
    private GameObject go;

    private Transform rangedAttackPosition;
    private Transform swingAttackPosition;
    private Transform aheadAttackPosition;//stessa posizione usata per il break
    private Transform AoEAttackPosition;
    private Vector3 scaleChange;
    private Vector3 originalPositionTarget;

    private float velocityAttraction = 25.0f;

    private bool cooldownRangedAttk = false;
    private bool cooldownRayAttk = false;
    private bool cooldownAoEAttk = false;
    private bool cooldownSwingAttk = false;
    private bool cooldownAheadAttk = false;
    private bool cooldownBreakAttk = false;

    public bool isAttacking = false;
    public bool isUsingAoE = false;

    public bool isShooting = false;
    public bool isAttracting = false;
    public bool isCastingAoE = false;


    
    private float timeCoolDownRangedAttack = 6.4f;
    private float timeCoolDownRayAttack = 10.4f;
    private float attractingRootDuration = 2.0f;
    private float timeBeforeCastAttracting = 0.5f;
    private float AoEDuration = 1.8f;
    private float timeCoolDownAoEAttack = 8.4f;
    private float timeCoolDownSwingAttack= 1.6f;
    private float timeCoolDownAheadAttack = 1.6f;
    private float timeCoolDownBreakAttack = 4.4f;

    

    public void Start()
    {
        //playersParty = FindObjectOfType<GameManager>().getPartyInGame();
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
        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            if (isShooting)
            {
                //go.transform.LookAt(new Vector3(originalPositionTarget.x, 1.0f, originalPositionTarget.z));
                go.transform.position += go.transform.forward * speedRangedAttk * Time.deltaTime;
                //go.GetComponent<Rigidbody>().MovePosition(go.transform.position+ go.transform.forward * speedRangedAttk * Time.deltaTime);
                //bisogna poi cambiare isShooting in false
                if((go.transform.position - transform.position).magnitude > (originalPositionTarget - transform.position).magnitude)
                {
                    isAttacking = false;
                    isShooting = false;
                    Destroy(go.gameObject);
                }
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
                    isAttacking = false;
                    targetPlayer.GetComponent<moreSpecificProfile>().publicAddRootStatus(attractingRootDuration);//root player

                    targetPlayer = null;
                    //target = "";
                    rayAttack();
                }
                if (null != targetPlayer)
                {
                    if (targetPlayer.tag.Equals("Tank"))
                    {

                        if (targetPlayer.transform.GetComponent<TankProfile>().shieldActive)
                        {
                            isAttracting = false;
                            //stessa cosa di prima per far vedere che fallisce
                            isAttacking = false;
                            targetPlayer.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(0);

                            targetPlayer = null;
                            //target = "";
                            Debug.Log("FALLITO RAY STA DIFENDENDO");
                        }
                    }
                    else if (targetPlayer.tag.Equals("Mage"))
                    {

                        if (targetPlayer.transform.GetComponent<MageProfile>().defenseActive)
                        {
                            isAttracting = false;
                            //stessa cosa di prima per far vedere che fallisce
                            isAttacking = false;
                            targetPlayer.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(0);

                            targetPlayer = null;
                            //target = "";
                            Debug.Log("FALLITO RAY STA DIFENDENDO");
                        }

                    }
                }


            }

            if (isCastingAoE)
            {
                go.transform.localScale += scaleChange * Time.deltaTime;
            }
        }
        else
        {
            if(null != go)
            {
                isShooting = false;
                isAttacking = false;
                targetPlayer = null;
                isAttracting = false;
                isCastingAoE = false;
                Destroy(go.gameObject);
            }
        }
       
    }      

    public Transform getSwingAttackPos()
    {
        return swingAttackPosition;
    }
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int rand = Random.Range(0, playersParty.Length);
            //targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            targetPlayer = playersParty[rand];
            
            instanceIDtarget = targetPlayer.GetInstanceID();
            Debug.Log("ID TARGET " + instanceIDtarget);
            isAttacking = true;
            StartCoroutine(timeBeforeCastRangedAttack());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            int rand = Random.Range(0, playersParty.Length);

            //targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            targetPlayer = playersParty[rand];
            instanceIDtarget = targetPlayer.GetInstanceID();
            isAttacking = true;
            StartCoroutine(timeBeforeCastRayAttack());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            int rand = Random.Range(0, playersParty.Length);
            //targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            targetPlayer = playersParty[rand];
            instanceIDtarget = targetPlayer.GetInstanceID();
            isAttacking = true;
            isUsingAoE = true; 
            StartCoroutine(timeBeforeCastSwingAttk());
            //swingAttack();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            int rand = Random.Range(0, playersParty.Length);

            //targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            targetPlayer = playersParty[rand];
            instanceIDtarget = targetPlayer.GetInstanceID();
            isAttacking = true;
           
            StartCoroutine(timeBeforeCastAheadAttk());
            //aheadAttack();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            int rand = Random.Range(0, playersParty.Length);
            //targetPlayer = FindObjectOfType<MageProfile>().gameObject;
            targetPlayer = playersParty[rand];
            instanceIDtarget = targetPlayer.GetInstanceID();
            isAttacking = true;
            StartCoroutine(timeBeforeCastBreakAttk());
            //breakAttack();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isAttacking = true;
            isUsingAoE = true;
            StartCoroutine(timeBeforeCastAoEAttk());
            //AoEAttack();
        }
    }*/


    public void hubAttacks(int attackCode, GameObject player)
    {

        isAttacking = true;

        switch (attackCode)
        {
            case 0://RANGED 
                targetPlayer = player;
                instanceIDtarget = targetPlayer.GetInstanceID();

                StartCoroutine(timeBeforeCastRangedAttack());
                break;
            case 1://RAY
                targetPlayer = player;
                instanceIDtarget = targetPlayer.GetInstanceID();

                StartCoroutine(timeBeforeCastRayAttack());
                break;
            case 2://SWING
                targetPlayer = player;
                instanceIDtarget = targetPlayer.GetInstanceID();
                isUsingAoE = true;

                StartCoroutine(timeBeforeCastSwingAttk());
                break;
            case 3://AHEAD
                targetPlayer = player;
                instanceIDtarget = targetPlayer.GetInstanceID();

                StartCoroutine(timeBeforeCastAheadAttk());
                break;
            case 4://BREAK
                targetPlayer = player;
                instanceIDtarget = targetPlayer.GetInstanceID();              

                StartCoroutine(timeBeforeCastBreakAttk());
                break;
            default://AoE
                isUsingAoE = true;

                StartCoroutine(timeBeforeCastAoEAttk());
                break;
        }

    }



    public IEnumerator timeBeforeCastRangedAttack()
    {
        //ricordarsi di gestire i cooldown
        originalPositionTarget = targetPlayer.transform.position;
        yield return new WaitForSeconds(timeBeforeCastAttracting);
        rangedAttack();
    }


    public void rangedAttack()
    {
        //target gia' scelto? o lo sceglie qua?se facessi due brain una per target una per azioni vere e do' reward combinato?
        //Debug.Log(" RANGED BOSS" + targetPlayer.transform.position);
        
        turnBossToTargetForRanged();
        go = Instantiate(goRangedAttk, rangedAttackPosition.position, transform.rotation, gameObject.transform);
        isShooting = true;
        go.transform.LookAt(new Vector3(originalPositionTarget.x, 2.0f, originalPositionTarget.z));

        cooldownRangedAttk = true;
        StartCoroutine(startCooldownRangedAttk());//gestire distruzione proiettile
    }

    public IEnumerator startCooldownRangedAttk()
    {
        //ricordarsi di gestire i cooldown
        yield return new WaitForSeconds(timeCoolDownRangedAttack);
        cooldownRangedAttk = false;
    }



   

    public IEnumerator timeBeforeCastRayAttack()
    {
        //ricordarsi di gestire i cooldown
        yield return new WaitForSeconds(timeBeforeCastAttracting);
        LaunchRay();
    }

    public void LaunchRay()
    {
        Debug.Log("HO ATTIVATO RAY");
        //turnBossToTarget();

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
            targetPlayer.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(1);
            targetPlayer.GetComponent<moreSpecificProfile>().impulseFromRay((transform.position - targetPlayer.GetComponent<Rigidbody>().transform.position).magnitude, m_PlayerMask);
        }
        else
        {
            //fai qualcosa epr far vedere che e' fallito il ray ma se sta difendendo bad reward
            targetPlayer = null;
            instanceIDtarget = 0;
            //target = "";
            Debug.Log("FALLITO RAY STA DIFENDENDO");
        }
           
    }

    public void rayAttack()
    {
        cooldownRayAttk = true;
        StartCoroutine(startCooldownRayAttk());
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
        //isAttacking = true;
        cooldownAoEAttk = true;
        StartCoroutine(castingAoEAttack());
        StartCoroutine(startCooldownAoEAttk());
    }

    public IEnumerator castingAoEAttack()
    {
        yield return new WaitForSeconds(AoEDuration);
        isCastingAoE = false;
        Destroy(go);
        isAttacking = false;
        isUsingAoE = false;

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

        
        //isAttacking = true;
        codeAttack = 0;
        StartCoroutine(waitBeforeRemoveSword(1));
        StartCoroutine(startCooldownSwingAttk());
    }

    public IEnumerator startCooldownSwingAttk()
    {
        cooldownSwingAttk = true;
        yield return new WaitForSeconds(timeCoolDownSwingAttack);
        cooldownSwingAttk = false;
    }

    public void aheadAttack()
    {
        turnBossToTarget();
        go = Instantiate(swordAheadAttk, aheadAttackPosition.position, transform.rotation, gameObject.transform);

        Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, go.transform.position.y, targetPlayer.transform.position.z);

        go.transform.LookAt(verticalAdj);
        //isAttacking = true;
        codeAttack = 1;
        StartCoroutine(waitBeforeRemoveSword(0));
        StartCoroutine(startCooldownAheadAttk());
    }

    public IEnumerator startCooldownAheadAttk()
    {
        cooldownAheadAttk = true;
        yield return new WaitForSeconds(timeCoolDownAheadAttack);
        cooldownAheadAttk = false;
    }

    public IEnumerator waitBeforeRemoveSword(int code)
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(go);
        isAttacking = false;
        targetPlayer = null;
        instanceIDtarget = 0;
        if (code == 1)
        {
            isUsingAoE = false;
        }
        //target = "";
    }

    public void breakAttack()//wounds that limits healing and reduce armor for tot sec.
    {
        turnBossToTarget();

        //isAttacking = true;
        go = Instantiate(swordAheadAttk, aheadAttackPosition.position, transform.rotation, gameObject.transform);//per il momento uguale a aheadAttack poi va cambiato

        Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, go.transform.position.y, targetPlayer.transform.position.z);

        go.transform.LookAt(verticalAdj);
        targetPlayer.GetComponent<moreSpecificProfile>().applyWound();//apply wounds
        targetPlayer.GetComponent<moreSpecificProfile>().reduceArmor();//apply armor reduction
        codeAttack = 2;
        //danno basso
        StartCoroutine(waitBeforeRemoveSword(0));
        StartCoroutine(startCooldownBreakAttk());
    }

    public IEnumerator startCooldownBreakAttk()
    {
        cooldownBreakAttk = true;
        yield return new WaitForSeconds(timeCoolDownBreakAttack);
        cooldownBreakAttk = false;
    }

    public IEnumerator timeBeforeCastSwingAttk()
    {
        yield return new WaitForSeconds(0.5f);
        swingAttack();
    }
    public IEnumerator timeBeforeCastAheadAttk()
    {
        yield return new WaitForSeconds(0.5f);
        aheadAttack();
    }
    public IEnumerator timeBeforeCastBreakAttk()
    {
        yield return new WaitForSeconds(0.5f);
        breakAttack();
    }

    public IEnumerator timeBeforeCastAoEAttk()
    {
        yield return new WaitForSeconds(0.5f);
        AoEAttack();
    }

    public void turnBossToTarget()
    {
        Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, transform.position.y, targetPlayer.transform.position.z);
      
        transform.LookAt(verticalAdj);
    }

    public void turnBossToTargetForRanged()
    {
        //Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, transform.position.y, targetPlayer.transform.position.z);
        Vector3 verticalAdj = new Vector3(originalPositionTarget.x, transform.position.y, originalPositionTarget.z);
        
        transform.LookAt(verticalAdj);
    }


    public void assignPartyForProfile()
    {
        playersParty = FindObjectOfType<GameManager>().getParty();
    }

    public void checkChampDieInFight()
    {
        playersParty = removeChampDieInFight();

        GetComponent<BossBehavior>().adjustPlayerArray(playersParty); // aggiorno array BOSS
        Debug.Log("ORA IL CONTEGGIO DEI GIOCATORI E'  "+ playersParty.Length);
    }
    
    public GameObject[] removeChampDieInFight()
    {
        GameObject[] arrNew = new GameObject[playersParty.Length - 1];
        int count = 0;

        for(int i=0; i<playersParty.Length; i++)
        {
            if (playersParty[i].GetComponent<moreSpecificProfile>().getStatusLifeChamp()!=1)
            {
                arrNew[count] = playersParty[i];
                count++;
            }
        }

        return arrNew;
    }

}


