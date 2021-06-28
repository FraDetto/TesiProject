using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBehavior : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    private GameObject boss;
    private Rigidbody rb;

    public float reactionTime = 1.2f;
    public float distanceRangeDown = 45.0f;
    public float distanceRangeUp = 50.0f;
    public bool firstRush = true;

    

    // Start is called before the first frame update
    void Start()
    {

        //boss = GameObject.FindGameObjectWithTag("Boss");
        rb = GetComponent<Rigidbody>();


        ////////// MAIN FSM ///////////////////
        FSMState takSafeSpot = new FSMState();
        takSafeSpot.enterActions.Add(takSafeSpotFromBoss);
        takSafeSpot.stayActions.Add(takSafeSpotFromBoss);



        FSMState Combact = new FSMState();
        Combact.enterActions.Add(combactFase);
        Combact.stayActions.Add(combactFase);

        // Define transitions
        FSMTransition t1 = new FSMTransition(safeSpotToCombact);
        FSMTransition t2 = new FSMTransition(CombactToSafeSpot);



        // Link states with transitions
        takSafeSpot.AddTransition(t1, Combact);
        Combact.AddTransition(t2, takSafeSpot);





        //////////// COMBACT FSM /////////////////
        FSMState Attack = new FSMState();
        Attack.enterActions.Add(AttackBoss);
        Attack.stayActions.Add(AttackBoss);

        FSMState Defend = new FSMState();
        Defend.enterActions.Add(DefendFromAttack);

        FSMState Special = new FSMState();
        Special.enterActions.Add(ActiveSpecial);

        // Define transitions

        FSMTransition t3 = new FSMTransition(AttkToSpec);
        FSMTransition t4 = new FSMTransition(AttkToDef);
        FSMTransition t5 = new FSMTransition(DefToAttk); // different from t1
        FSMTransition t6 = new FSMTransition(DefToSpec);
        FSMTransition t7 = new FSMTransition(SpecToAttk);
        FSMTransition t8 = new FSMTransition(SpecToDef);


        // Link states with transitions
        Attack.AddTransition(t3, Special);
        Attack.AddTransition(t4, Defend);


        Defend.AddTransition(t5, Attack);
        Defend.AddTransition(t6, Special);

        Special.AddTransition(t7, Attack);
        Special.AddTransition(t8, Defend);

        // Setup a FSA at initial state
        fsmCombact = new FSM(Attack);


        // Setup a FSA at initial state
        fsmMain = new FSM(takSafeSpot);


        //Debug.Log("MAGE " + this.gameObject.GetInstanceID());

        // Start monitoring
        StartCoroutine(Fight());
    }




    // Periodic update, run forever
    public IEnumerator Fight()
    {
        //while (true)
        while(GetComponent<moreSpecificProfile>().getStatusLifeChamp()==0 && GetComponent<moreSpecificProfile>().flagResetepisode ==false)
        {
            fsmMain.Update();
            yield return new WaitForSeconds(reactionTime);
        }
        ////Animation death
        transform.rotation = Quaternion.Euler(new Vector3(90f, 0.0f, 0f));
    }




    /////////////////// MAIN FSM ////////////////////////////////

    // CONDITIONS

    public bool safeSpotToCombact()
    {
        if ( ((boss.transform.position - rb.transform.position).magnitude >= distanceRangeDown && (boss.transform.position - rb.transform.position).magnitude<= distanceRangeUp && GetComponent<moreSpecificProfile>().publicGetStatus() != 2) || GetComponent<moreSpecificProfile>().publicGetStatus() == 1)
        {
            GetComponent<MageMovement>().distanceFlag = false;
            GetComponent<MageMovement>().chaseFlag = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CombactToSafeSpot()
    {
        //Debug.Log(" COMABCT TO SAFE " + ((boss.transform.position - rb.transform.position).magnitude >= distanceRangeDown && (boss.transform.position - rb.transform.position).magnitude <= distanceRangeUp)  + "  STATUS ROOT " + (GetComponent<moreSpecificProfile>().publicGetStatus() == 1) + " VALORE SAFESPOT " + !safeSpotToCombact());
        if (GetComponent<moreSpecificProfile>().publicGetStatus() == 0)//If status OK work normal, otherwise FALSE -> Rooted or stunned can't move
        {
            return !safeSpotToCombact();
        }
        else
        {
            return false;
        }
       
    }
    

    // ACTIONS

    public void takSafeSpotFromBoss()//allontanati dal boss
    {
        //Debug.Log("CONTROLLO DOVE DOVREBBE SETTARE MOVIMENTO DASHING " + GetComponent<MageProfile>().isDashing + " CHASE FLAG " + GetComponent<MageMovement>().chaseFlag);
        if (!GetComponent<MageProfile>().isDashing)
        {
            if ((boss.transform.position - rb.transform.position).magnitude < distanceRangeDown)
            {
                if (!GetComponent<MageMovement>().distanceFlag)
                {
                    GetComponent<MageMovement>().distanceFlag = true;
                }
            }
            else
            {
                if (!GetComponent<MageMovement>().chaseFlag)
                {
                    GetComponent<MageMovement>().chaseFlag = true;
                }
            }
        }
        


        
    }



    public void combactFase()
    {

        if (GetComponent<moreSpecificProfile>().publicGetStatus() != 2)//if 2 = stunned can't attack and move
        {
            fsmCombact.Update();
        }
        
    }

    /*
    public void chaseDistanceBoss()
    {
        if (!GetComponent<MageMovement>().chaseFlag && GetComponent<moreSpecificProfile>().publicGetStatus() == 0)
        {
            GetComponent<MageMovement>().chaseFlag = true;
        }
    }
    */

    //////////////////// COMBACT FSM //////////////////////////////

    // CONDITIONS


    public bool AttkToDef()
    {
        
        if ( (!GetComponent<MageProfile>().cooldownDefense || (!GetComponent<MageProfile>().cooldownDash && GetComponent<moreSpecificProfile>().publicGetStatus()==0) ) && ( boss.GetComponentInChildren<BossAttackBehavior>().isAttacking && ( (boss.GetComponentInChildren<BossBehavior>().instanceIDtarget == this.gameObject.GetInstanceID() && (attackInRange() || boss.GetComponentInChildren<BossAttackBehavior>().isShooting) ) || (boss.GetComponentInChildren<BossAttackBehavior>().isUsingAoE && attackInRange()) )   ))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool AttkToSpec()
    {
        if (!GetComponent<MageProfile>().cooldownSpecial)
        {
            //Debug.Log("ATTKSPEC");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DefToAttk()
    {
        if (GetComponent<MageProfile>().cooldownSpecial && !GetComponent<MageProfile>().defenseActive)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DefToSpec()
    {
        if (!GetComponent<MageProfile>().cooldownSpecial && !GetComponent<MageProfile>().defenseActive)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SpecToAttk()
    {
        if (!GetComponent<MageProfile>().chargingUlt && ( GetComponent<MageProfile>().cooldownDefense || ( GetComponent<moreSpecificProfile>().publicGetStatus() == 1) ||
            (!boss.GetComponentInChildren<BossAttackBehavior>().isAttacking || boss.GetComponentInChildren<BossBehavior>().instanceIDtarget != this.gameObject.GetInstanceID() || (boss.GetComponentInChildren<BossAttackBehavior>().isUsingAoE && !attackInRange()) ) ))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SpecToDef()
    {
        if (!GetComponent<MageProfile>().chargingUlt  && (!GetComponent<MageProfile>().cooldownDefense || (!GetComponent<MageProfile>().cooldownDash && GetComponent<moreSpecificProfile>().publicGetStatus() == 0) ) &&
            (boss.GetComponentInChildren<BossAttackBehavior>().isAttacking && ((boss.GetComponentInChildren<BossBehavior>().instanceIDtarget == this.gameObject.GetInstanceID() && attackInRange()) || boss.GetComponentInChildren<BossAttackBehavior>().isUsingAoE || boss.GetComponentInChildren<BossAttackBehavior>().isShooting)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // ACTIONS

    public void AttackBoss()
    {
        if (!firstRush && GetComponent<moreSpecificProfile>().flagResetepisode == false)
        {
           // Debug.Log("ATTACK BOSS !FIRSTRUSH");
            GetComponent<MageProfile>().attackWithMagic();
        }
        else
        {
           //Debug.Log("ATTACK BOSS FIRST RUSH");
            firstRush = false;
        }
    }

    public void DefendFromAttack()
    {
        
        if ((!GetComponent<MageProfile>().cooldownDefense && (!GetComponent<MageProfile>().cooldownDash && GetComponent<moreSpecificProfile>().publicGetStatus() == 0) ))
        {
            //qua probabilita'
            int random = Random.Range(1, 101);

            if (random >= 1 && random < 60)
            {
                GetComponent<MageProfile>().defendWithSpell();
            }
            else
            {
                GetComponent<MageProfile>().rollAway();
            }
        }
        else
        {
            if (!GetComponent<MageProfile>().cooldownDefense)
            {
                GetComponent<MageProfile>().defendWithSpell();
            }
            else
            {
                GetComponent<MageProfile>().rollAway();
            }
        }
    }

    public void ActiveSpecial()
    {
        GetComponent<MageProfile>().activateUlti();
    }


    public bool attackInRange()
    {
        if ((boss.transform.position - rb.transform.position).magnitude < 12.0f)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void setBoss(GameObject bo)
    {
        boss = bo;
    }

}
