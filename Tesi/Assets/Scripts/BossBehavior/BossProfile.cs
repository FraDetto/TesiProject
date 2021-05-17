using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProfile : moreSpecificProfile
{
    
    public GameObject[] playersParty;
    public float speedRangedAttk = 35.0f;

    public int codeAttack = 0; // 0:swing  1: ahead  2:break    i use it to divide the different attack and apply the correct damage

    public GameObject goRangedAttk;
    public GameObject swordSwingAttk;
    public GameObject swordAheadAttk;
    public GameObject containerAoEAttk;

    public LayerMask m_PlayerMask;

    private Rigidbody rb;
    public GameObject targetPlayer;

    public GameObject targetPlayerForRay;


    public int instanceIDtarget;
    //public string target;
    private GameObject go;
    private GameObject goRanged;
    private GameObject goSwing;
    private GameObject goAhead;
    private GameObject goAoE;

    private Transform rangedAttackPosition;
    private Transform swingAttackPosition;
    private Transform aheadAttackPosition;//stessa posizione usata per il break
    private Transform AoEAttackPosition;
    private Vector3 scaleChange;
    private Vector3 originalPositionTarget;

    private float velocityAttraction = 26.0f;

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
    private float timeBeforeCastAttracting = 0.4f;
    private float AoEDuration = 1.2f;
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

        scaleChange = new Vector3(0.16f, 0.018f, 0.16f);

    }


    private void FixedUpdate()
    {
        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            if (isShooting)
            {
                //go.transform.LookAt(new Vector3(originalPositionTarget.x, 1.0f, originalPositionTarget.z));
                goRanged.transform.position += goRanged.transform.forward * speedRangedAttk * Time.deltaTime;
                //go.GetComponent<Rigidbody>().MovePosition(go.transform.position+ go.transform.forward * speedRangedAttk * Time.deltaTime);
                //bisogna poi cambiare isShooting in false
                if((goRanged.transform.position - transform.position).magnitude > (originalPositionTarget - transform.position).magnitude)
                {
                    isAttacking = false;
                    isShooting = false;
                    Destroy(goRanged.gameObject);
                }
            }


            if (isAttracting)
            {

                Vector3 verticalAdjBoss = new Vector3(targetPlayerForRay.transform.position.x, transform.position.y, targetPlayerForRay.transform.position.z);
                Vector3 verticalAdj = new Vector3(transform.position.x, targetPlayerForRay.transform.position.y, transform.position.z);
                Vector3 toBossPos = (verticalAdj - targetPlayerForRay.transform.position);

                if ((transform.position - targetPlayerForRay.GetComponent<Rigidbody>().transform.position).magnitude > 8.0f)
                {
                    transform.LookAt(verticalAdjBoss);
                    targetPlayerForRay.transform.LookAt(verticalAdj);
                    targetPlayerForRay.GetComponent<Rigidbody>().MovePosition(targetPlayerForRay.transform.position + (targetPlayerForRay.transform.forward) * velocityAttraction * Time.deltaTime);
                }
                else
                {
                    isAttracting = false;
                    isAttacking = false;
                    targetPlayerForRay.GetComponent<moreSpecificProfile>().publicAddRootStatus(attractingRootDuration);//root player

                    targetPlayerForRay = null;
                    //target = "";
                    rayAttack();
                }
                if (null != targetPlayerForRay)
                {
                    if (targetPlayerForRay.tag.Equals("Tank"))
                    {

                        if (targetPlayerForRay.transform.GetComponent<TankProfile>().shieldActive)
                        {
                            isAttracting = false;
                            //stessa cosa di prima per far vedere che fallisce
                            isAttacking = false;
                            targetPlayerForRay.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(0);

                            targetPlayerForRay = null;
                            //target = "";
                            Debug.Log("FALLITO RAY STA DIFENDENDO");
                        }
                    }
                    else if (targetPlayerForRay.tag.Equals("Mage"))
                    {

                        if (targetPlayerForRay.transform.GetComponent<MageProfile>().defenseActive)
                        {
                            isAttracting = false;
                            //stessa cosa di prima per far vedere che fallisce
                            isAttacking = false;
                            targetPlayerForRay.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(0);

                            targetPlayerForRay = null;
                            //target = "";
                            Debug.Log("FALLITO RAY STA DIFENDENDO");
                        }

                    }
                }


            }

            if (isCastingAoE)
            {
                goAoE.transform.localScale += scaleChange * Time.deltaTime;
            }
        }
        else
        {
            if(null != go)
            {
                isShooting = false;
                isAttracting = false;
                Destroy(go.gameObject);
            }
            if(null != goAoE)
            {
                isAttracting = false;
                isCastingAoE = false;
                Destroy(goAoE.gameObject);
            }
            if(null != goRanged)
            {
                isShooting = false;

                Destroy(goRanged.gameObject);
            }
            if(null != goAhead)
            {
                Destroy(goAhead.gameObject);
            }
            isAttacking = false;
            targetPlayer = null;
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

        Debug.Log("PLAYER PER PROSSIMO ATTACCO " + player.tag);

        switch (attackCode)
        {
            case 0://RANGED 
                targetPlayer = player;
                instanceIDtarget = targetPlayer.GetInstanceID();

                StartCoroutine(timeBeforeCastRangedAttack());
                break;
            case 1://RAY
                targetPlayerForRay = player;
                instanceIDtarget = targetPlayerForRay.GetInstanceID();

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

        if (null != goRanged)
        {
            Debug.Log(" go RANGED ERA PIENO LO DISTRUGGO");
            Destroy(goRanged);
        }

        turnBossToTargetForRanged();
        goRanged = Instantiate(goRangedAttk, rangedAttackPosition.position, transform.rotation, gameObject.transform);
        isShooting = true;
        goRanged.transform.LookAt(new Vector3(originalPositionTarget.x, 2.0f, originalPositionTarget.z));

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
        Debug.Log("HO ATTIVATO RAY " + targetPlayerForRay);
        Debug.Log("SU NOME: " + targetPlayerForRay.tag);
        //turnBossToTarget();

        bool enemyIsDefending;

        switch (targetPlayerForRay.tag)
        {
            case "Tank":
                enemyIsDefending = targetPlayerForRay.transform.GetComponent<TankProfile>().shieldActive;

                break;

            case "Mage":
                enemyIsDefending = targetPlayerForRay.transform.GetComponent<MageProfile>().defenseActive;
                break;

            default:
                enemyIsDefending = false;
                break;
        }
        if (!enemyIsDefending)
        {
            isAttracting = true;
            targetPlayerForRay.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(1);
            targetPlayerForRay.GetComponent<moreSpecificProfile>().impulseFromRay((transform.position - targetPlayerForRay.GetComponent<Rigidbody>().transform.position).magnitude, m_PlayerMask);
        }
        else
        {
            //fai qualcosa epr far vedere che e' fallito il ray ma se sta difendendo bad reward
            targetPlayerForRay = null;
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
        goAoE = Instantiate(containerAoEAttk, AoEAttackPosition.position, transform.rotation, gameObject.transform);
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
        isAttacking = false;
        isUsingAoE = false;
        Destroy(goAoE);

    }

    public IEnumerator startCooldownAoEAttk()
    {
        yield return new WaitForSeconds(timeCoolDownAoEAttack);
        cooldownAoEAttk = false;
    }

    public void swingAttack()
    {
        Debug.Log("HO ATTIVATO SWING " + targetPlayer);
        Debug.Log("SU NOME: " + targetPlayer.tag);

        if (null != goSwing)
        {
            Debug.Log(" go SWING ERA PIENO LO DISTRUGGO");
            Destroy(goSwing);
        }

        turnBossToTarget();
        goSwing = Instantiate(swordSwingAttk, swingAttackPosition.position, transform.rotation, gameObject.transform);

        
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
        

        if(null != goAhead)
        {
            Debug.Log(" go AhEAD ERA PIENO LO DISTRUGGO");
            Destroy(goAhead);
        }
        turnBossToTarget();
        goAhead = Instantiate(swordAheadAttk, aheadAttackPosition.position, transform.rotation, gameObject.transform);

        Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, goAhead.transform.position.y, targetPlayer.transform.position.z);

        goAhead.transform.LookAt(verticalAdj);
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

        isAttacking = false;
        targetPlayer = null;
        instanceIDtarget = 0;
        if (code == 1)
        {
            Destroy(goSwing.gameObject);
            isUsingAoE = false;
        }
        else
        {
            if(null != goAhead)
            {
                Destroy(goAhead.gameObject);
            }
            else
            {
                Destroy(go.gameObject);
            }
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
        yield return new WaitForSeconds(0.4f);
        swingAttack();
    }
    public IEnumerator timeBeforeCastAheadAttk()
    {
        yield return new WaitForSeconds(0.4f);
        aheadAttack();
    }
    public IEnumerator timeBeforeCastBreakAttk()
    {
        yield return new WaitForSeconds(0.4f);
        breakAttack();
    }

    public IEnumerator timeBeforeCastAoEAttk()
    {
        yield return new WaitForSeconds(0.4f);
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
        Debug.Log("ORA IL CONTEGGIO DEI GIOCATORI E'  " + playersParty.Length);
        GetComponent<BossBehavior>().adjustPlayerArray(playersParty); // aggiorno array BOSS
        
    }
    
    public GameObject[] removeChampDieInFight()
    {
        if (playersParty.Length==1)
        {
            return null;
        }
        else
        {
            GameObject[] arrNew = new GameObject[playersParty.Length - 1];
            int count = 0;

            for (int i = 0; i < playersParty.Length; i++)
            {
                if (playersParty[i].GetComponent<moreSpecificProfile>().getStatusLifeChamp() != 1)
                {
                    arrNew[count] = playersParty[i];
                    count++;
                }
            }

            return arrNew;
        }
        
    }

}


