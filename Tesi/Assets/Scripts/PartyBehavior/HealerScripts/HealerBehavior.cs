using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerBehavior : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    private GameObject boss;
    private Rigidbody rb;

    public float reactionTime = 2.5f;
    public float distanceRangeDown = 45.0f;
    public float distanceRangeUp = 60.0f;
    public bool firstRush = true;

    public float m_HealRadius = 80f;
    public LayerMask m_PlayerMask;
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

        FSMState Heal = new FSMState();
        Heal.enterActions.Add(HealAnAlly);

        FSMState Special = new FSMState();
        Special.enterActions.Add(ActiveSpecial);

        // Define transitions

        FSMTransition t3 = new FSMTransition(AttkToSpec);
        FSMTransition t4 = new FSMTransition(AttkToHeal);
        FSMTransition t5 = new FSMTransition(HealToAttk); // different from t1
        FSMTransition t6 = new FSMTransition(HealToSpec);
        FSMTransition t7 = new FSMTransition(SpecToAttk);
        FSMTransition t8 = new FSMTransition(SpecToHeal);


        // Link states with transitions
        Attack.AddTransition(t3, Special);
        Attack.AddTransition(t4, Heal);


        Heal.AddTransition(t5, Attack);
        Heal.AddTransition(t6, Special);

        Special.AddTransition(t7, Attack);
        Special.AddTransition(t8, Heal);

        // Setup a FSA at initial state
        fsmCombact = new FSM(Attack);


        // Setup a FSA at initial state
        fsmMain = new FSM(takSafeSpot);

        //Debug.Log("HEAL " + this.gameObject.GetInstanceID());

        // Start monitoring
        StartCoroutine(Fight());
    }




    // Periodic update, run forever
    public IEnumerator Fight()
    {
        while (GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0 && GetComponent<moreSpecificProfile>().flagResetepisode==false)
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
        if ( ((boss.transform.position - rb.transform.position).magnitude >= distanceRangeDown && (boss.transform.position - rb.transform.position).magnitude <= distanceRangeUp && GetComponent<moreSpecificProfile>().publicGetStatus() != 2) || GetComponent<moreSpecificProfile>().publicGetStatus() == 1)
        {
            GetComponent<HealerMovement>().distanceFlag = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CombactToSafeSpot()
    {
        if (GetComponent<HealerProfile>().publicGetStatus() == 0)//If status OK work normal, otherwise FALSE -> Rooted or stunned can't move
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
        if ((boss.transform.position - rb.transform.position).magnitude < distanceRangeDown)
        {
            if (!GetComponent<HealerMovement>().distanceFlag)
            {
                GetComponent<HealerMovement>().distanceFlag = true;
            }
        }
        else
        {
            if (!GetComponent<HealerMovement>().chaseFlag)
            {
                GetComponent<HealerMovement>().chaseFlag = true;
            }
        }

    }



    public void combactFase()
    {
        //Debug.Log("Combact Fase HEALER");
        if (GetComponent<HealerProfile>().publicGetStatus() != 2)//if 2 = stunned can't attack and move
        {
            fsmCombact.Update();
        }
    }



    //////////////////// COMBACT FSM //////////////////////////////

    // CONDITIONS


    public bool AttkToHeal()
    {
        if ( ( !GetComponent<HealerProfile>().cooldownHeal && !allFullLife() && (GetComponent<HealerProfile>().cooldownSpecial || !boss.GetComponent<BossAttackBehavior>().isUsingAoE) ) ||
            ( (!GetComponent<HealerProfile>().cooldownDash && GetComponent<moreSpecificProfile>().publicGetStatus() == 0) && (boss.GetComponent<BossAttackBehavior>().isAttacking && ( (boss.GetComponent<BossBehavior>().instanceIDtarget == this.gameObject.GetInstanceID() && (attackInRange() || boss.GetComponent<BossAttackBehavior>().isShooting) ) || (boss.GetComponent<BossAttackBehavior>().isUsingAoE && attackInRange())  ))) )
        {
            //Debug.Log("SONO IN VADo HEAL");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AttkToSpec()
    {
        if ( !GetComponent<HealerProfile>().cooldownSpecial && boss.GetComponent<BossAttackBehavior>().isUsingAoE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HealToAttk()
    {
        if ( GetComponent<HealerProfile>().cooldownSpecial || !boss.GetComponent<BossAttackBehavior>().isUsingAoE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HealToSpec()
    {
        if (!GetComponent<HealerProfile>().cooldownSpecial && boss.GetComponent<BossAttackBehavior>().isUsingAoE)
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
        if ( GetComponent<HealerProfile>().cooldownHeal || allFullLife())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SpecToHeal()
    {
        if (!GetComponent<HealerProfile>().cooldownHeal && !allFullLife())
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
            GetComponent<HealerProfile>().attackWithMagic();
        }
        else
        {
            //Debug.Log("ATTACK BOSS FIRST RUSH");
            firstRush = false;
        }
    }

    public void HealAnAlly()
    {
        //si da' priorita' a cura
        if (!GetComponent<HealerProfile>().cooldownHeal && !allFullLife() )
        {
            GetComponent<HealerProfile>().healAlly();
        }
        else
        {
            GetComponent<HealerProfile>().rollAway();
        }
       
    }

    public void ActiveSpecial()
    {
        GetComponent<HealerProfile>().activateUlti();
    }

    public bool allFullLife()
    {
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_HealRadius, m_PlayerMask);
       
        bool flag = true;
        for (int i = 0; i < colliders.Length; i++)
        {
            moreSpecificProfile targetProfile = colliders[i].GetComponent<moreSpecificProfile>();
            if (!targetProfile)
                continue;

            if (targetProfile.getStatusLifeChamp() == 0)
            {
                if (targetProfile.publicGetCurrentLife() != targetProfile.publicGetTotalLife())
                {
                    //Debug.Log("Trovato uno ferito");
                    flag = false;
                }
            }
               

        }

        return flag;
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
