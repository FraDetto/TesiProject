using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BossAttackBehavior : Agent
{
    //public GameObject[] playersParty;
    public float speedRangedAttk = 35.0f;

    public int codeAttack = 0; // 0:swing  1: ahead  2:break    i use it to divide the different attack and apply the correct damage

    public GameObject goRangedAttk;
    public GameObject swordSwingAttk;
    public GameObject swordAheadAttk;
    public GameObject containerAoEAttk;

    public LayerMask m_PlayerMask;
    public GameManager gameManager;

    //public LayerMask m_PlayerMask;

    public Rigidbody rb;
    public GameObject targetPlayer;

    public GameObject targetPlayerForRay;
    

    public int instanceIDtarget;
    //public string target;
    private GameObject goBreak;
    private GameObject goRanged;
    private GameObject goSwing;
    private GameObject goAhead;
    private GameObject goAoE;

    public Transform rangedAttackPosition;
    public Transform swingAttackPosition;
    public Transform aheadAttackPosition;//stessa posizione usata per il break
    public Transform AoEAttackPosition;
    //public GameManager gameManager;

    private Vector3 scaleChange = new Vector3(0.16f, 0.018f, 0.16f);
    private Vector3 originalPositionTarget;

    private float velocityAttraction = 26.0f;

    public bool isAttacking = false;
    public bool isUsingAoE = false;

    public bool isShooting = false;
    public bool isAttracting = false;
    public bool isCastingAoE = false;

    private float AoEDuration = 1.2f;
    private float attractingRootDuration = 2.0f;
    private float timeBeforeCastAttracting = 0.4f;


    private GameObject reserveVarTarget;
    private int reserveVarIDtarget;



    private bool chainRanged = false;
    private bool chainRay = false;







    public override void OnActionReceived(float[] vectorAction)
    {
        Debug.Log(" =====OnActionReceived===== " + " ATTACK " + vectorAction[0]);
        /*
        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            isAttacking = true;

            Debug.Log("PLAYER PER PROSSIMO ATTACCO " + player.tag + " ID " + player.GetInstanceID());
            reserveVarIDtarget = player.GetInstanceID();
            reserveVarTarget = player;
            //Debug.Log("PLAYER RISERVA " + reserveVarTarget.tag + " ID " + reserveVarIDtarget);

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
                    Debug.Log("SONO IN RAY IN HUB " + targetPlayerForRay + " ID CON " + instanceIDtarget);
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
                    Debug.Log("SONO IN BREAK IN HUB " + targetPlayer + " ID CON " + instanceIDtarget);
                    StartCoroutine(timeBeforeCastBreakAttk());
                    break;
                default://AoE
                    isUsingAoE = true;

                    StartCoroutine(timeBeforeCastAoEAttk());
                    break;
            }
        }
        else
        {
            Debug.Log("SONO IN HUB MA EP FINITPO " + GetComponent<moreSpecificProfile>().flagResetepisode);

        }
        */
    }


    private void FixedUpdate()
    {
        
        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            if (isShooting)
            {
                goRanged.transform.position += goRanged.transform.forward * speedRangedAttk * Time.deltaTime;

                if ((goRanged.transform.position - transform.position).magnitude > (originalPositionTarget - transform.position).magnitude)
                {
                    isAttacking = false;
                    isShooting = false;
                    Destroy(goRanged.gameObject);
                }
            }


            if (isAttracting)
            {

                if ((transform.position - targetPlayerForRay.GetComponent<Rigidbody>().transform.position).magnitude > 8.0f)
                {

                    targetPlayerForRay.GetComponent<Rigidbody>().MovePosition(targetPlayerForRay.transform.position + (targetPlayerForRay.transform.forward) * velocityAttraction * Time.deltaTime);
                }
                else
                {
                    isAttracting = false;
                    isAttacking = false;
                    targetPlayerForRay.GetComponent<moreSpecificProfile>().publicAddRootStatus(attractingRootDuration);//root player

                    targetPlayerForRay = null;
                }
                if (null != targetPlayerForRay)
                {
                    if (targetPlayerForRay.tag.Equals("Tank"))
                    {

                        if (targetPlayerForRay.transform.GetComponent<TankProfile>().shieldActive)
                        {
                            isAttracting = false;

                            isAttacking = false;
                            targetPlayerForRay.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(0);

                            targetPlayerForRay = null;

                        }
                    }
                    else if (targetPlayerForRay.tag.Equals("Mage"))
                    {

                        if (targetPlayerForRay.transform.GetComponent<MageProfile>().defenseActive)
                        {
                            isAttracting = false;

                            isAttacking = false;
                            targetPlayerForRay.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(0);

                            targetPlayerForRay = null;

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
            if (null != goBreak)
            {
                isShooting = false;
                isAttracting = false;
                Destroy(goBreak.gameObject);
            }
            if (null != goAoE)
            {
                isAttracting = false;
                isCastingAoE = false;
                Destroy(goAoE.gameObject);
            }
            if (null != goRanged)
            {
                isShooting = false;

                Destroy(goRanged.gameObject);
            }
            if (null != goAhead)
            {
                Destroy(goAhead.gameObject);
            }
            isAttacking = false;
            targetPlayer = null;
        }
        
    }



    public IEnumerator timeBeforeCastRangedAttack()
    {
        //ricordarsi di gestire i cooldown
        originalPositionTarget = targetPlayer.transform.position;
        yield return new WaitForSeconds(timeBeforeCastAttracting);
        removePreviousGo();

        rangedAttack();


    }


    public void rangedAttack()
    {
        //target gia' scelto? o lo sceglie qua?se facessi due brain una per target una per azioni vere e do' reward combinato?
        //Debug.Log(" RANGED BOSS" + targetPlayer.transform.position);
        if (null != goRanged)
        {
            Destroy(goRanged.gameObject);
        }

        turnBossToTargetForRanged();
        goRanged = Instantiate(goRangedAttk, rangedAttackPosition.position, transform.rotation, gameObject.transform);
        isShooting = true;
        goRanged.transform.LookAt(new Vector3(originalPositionTarget.x, 2.0f, originalPositionTarget.z));
    }







    public IEnumerator timeBeforeCastRayAttack()
    {
        //ricordarsi di gestire i cooldown
        yield return new WaitForSeconds(timeBeforeCastAttracting);

        removePreviousGo();
        LaunchRay();


    }

    public void LaunchRay()
    {

        if (targetPlayerForRay == null)
        {
            targetPlayerForRay = reserveVarTarget;
            instanceIDtarget = reserveVarIDtarget;
        }
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
            Vector3 verticalAdj = new Vector3(transform.position.x, targetPlayerForRay.transform.position.y, transform.position.z);
            Vector3 toBossPos = (verticalAdj - targetPlayerForRay.transform.position);
            targetPlayerForRay.transform.LookAt(verticalAdj);


            isAttracting = true;
            targetPlayerForRay.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(1);
            targetPlayerForRay.GetComponent<moreSpecificProfile>().impulseFromRay((transform.position - targetPlayerForRay.GetComponent<Rigidbody>().transform.position).magnitude, m_PlayerMask);
        }
        else
        {
            targetPlayerForRay = null;
            instanceIDtarget = 0;
            //Debug.Log("FALLITO RAY STA DIFENDENDO");
        }

    }


    public void AoEAttack()
    {

        goAoE = Instantiate(containerAoEAttk, AoEAttackPosition.position, transform.rotation, gameObject.transform);
        isCastingAoE = true;
        StartCoroutine(castingAoEAttack());
    }

    public IEnumerator castingAoEAttack()
    {
        yield return new WaitForSeconds(AoEDuration);
        isCastingAoE = false;
        isAttacking = false;
        isUsingAoE = false;
        Destroy(goAoE);

    }


    public void swingAttack()
    {
        goSwing = Instantiate(swordSwingAttk, swingAttackPosition.position, transform.rotation, gameObject.transform);

        codeAttack = 0;
        StartCoroutine(waitBeforeRemoveSword(1));
    }

    public void aheadAttack()
    {
        if (targetPlayer == null)
        {
            targetPlayer = reserveVarTarget;
            instanceIDtarget = reserveVarIDtarget;
        }
        goAhead = Instantiate(swordAheadAttk, aheadAttackPosition.position, transform.rotation, gameObject.transform);

        Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, goAhead.transform.position.y, targetPlayer.transform.position.z);

        goAhead.transform.LookAt(verticalAdj);
        codeAttack = 1;
        StartCoroutine(waitBeforeRemoveSword(0));
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
            if (null != goAhead)
            {
                Destroy(goAhead.gameObject);
            }
            else
            {
                Destroy(goBreak.gameObject);
            }
        }
    }

    public void breakAttack()//wounds that limits healing and reduce armor for tot sec.
    {
        if (targetPlayer == null)
        {
            targetPlayer = reserveVarTarget;
            instanceIDtarget = reserveVarIDtarget;
        }

        goBreak = Instantiate(swordAheadAttk, aheadAttackPosition.position, transform.rotation, gameObject.transform);



        Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, goBreak.transform.position.y, targetPlayer.transform.position.z);

        goBreak.transform.LookAt(verticalAdj);
        targetPlayer.GetComponent<moreSpecificProfile>().applyWound();//apply wounds
        targetPlayer.GetComponent<moreSpecificProfile>().reduceArmor();//apply armor reduction
        codeAttack = 2;
        //danno basso
        StartCoroutine(waitBeforeRemoveSword(0));
    }


    public IEnumerator timeBeforeCastSwingAttk()
    {
        yield return new WaitForSeconds(0.4f);
        removePreviousGo();
        swingAttack();

    }
    public IEnumerator timeBeforeCastAheadAttk()
    {
        yield return new WaitForSeconds(0.4f);
        removePreviousGo();
        aheadAttack();

    }
    public IEnumerator timeBeforeCastBreakAttk()
    {
        yield return new WaitForSeconds(0.4f);
        removePreviousGo();
        breakAttack();
    }

    public IEnumerator timeBeforeCastAoEAttk()
    {
        yield return new WaitForSeconds(0.4f);
        removePreviousGo();
        AoEAttack();

    }

    public void turnBossToTargetForRanged()
    {
        Vector3 verticalAdj = new Vector3(originalPositionTarget.x, transform.position.y, originalPositionTarget.z);

        transform.LookAt(verticalAdj);
    }


    public void endEpStopAll()
    {
        isCastingAoE = false;
        isAttracting = false;
        isAttacking = false;
        StopAllCoroutines();


    }


    public void removePreviousGo()
    {
        if (null != goSwing)
        {
            Destroy(goSwing.gameObject);
        }
        if (null != goAhead)
        {
            Destroy(goAhead.gameObject);
        }
        if (null != goBreak)
        {
            Destroy(goBreak.gameObject);
        }
        if (null != goAoE)
        {
            Destroy(goAoE.gameObject);
        }
    }



}
