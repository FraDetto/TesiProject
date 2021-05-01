using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserBehavior : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    private GameObject boss;
    private Rigidbody myRB;

    public float reactionTime = 1.2f;
    public float distanceRange = 7.0f;
    public bool firstRush = true;

    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        myRB = transform.GetComponent<Rigidbody>();


        ////////// MAIN FSM ///////////////////
        FSMState Chase = new FSMState();
        Chase.enterActions.Add(ChaseBoos);
        Chase.stayActions.Add(ChaseBoos);


        FSMState Combact = new FSMState();
        Combact.enterActions.Add(combactFase);
        Combact.stayActions.Add(combactFase);

        // Define transitions
        FSMTransition t1 = new FSMTransition(ChaseToCombact);
        FSMTransition t2 = new FSMTransition(CombactToChase);


        // Link states with transitions
        Chase.AddTransition(t1, Combact);
        Combact.AddTransition(t2, Chase);
       





        //////////// COMBACT FSM /////////////////
        FSMState Attack = new FSMState();
        Attack.enterActions.Add(AttackBoss);
        Attack.stayActions.Add(AttackBoss);

        FSMState HealHimself = new FSMState();
        HealHimself.enterActions.Add(Heal);

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
        Attack.AddTransition(t4, HealHimself);


        HealHimself.AddTransition(t5, Attack);
        HealHimself.AddTransition(t6, Special);

        Special.AddTransition(t7, Attack);
        Special.AddTransition(t8, HealHimself);

        // Setup a FSA at initial state
        fsmCombact = new FSM(Attack);

        // Setup a FSA at initial state
        fsmMain = new FSM(Chase);

        Debug.Log("BRUISER " + this.gameObject.GetInstanceID());

        // Start monitoring
        StartCoroutine(Fight());
    }




    // Periodic update, run forever
    public IEnumerator Fight()
    {
        while (GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0)
        {
            fsmMain.Update();
            yield return new WaitForSeconds(reactionTime);
        }
        ////Animation death
        transform.rotation = Quaternion.Euler(new Vector3(90f, 0.0f, 0f));
    }




    /////////////////// MAIN FSM ////////////////////////////////

    // CONDITIONS

    public bool ChaseToCombact()
    {
        if ( ((boss.transform.position - myRB.transform.position).magnitude <= distanceRange && GetComponent<moreSpecificProfile>().publicGetStatus() != 2) || GetComponent<moreSpecificProfile>().publicGetStatus() == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CombactToChase()
    {
        if (GetComponent<BruiserProfile>().publicGetStatus() == 0)//If status OK work normal, otherwise FALSE -> Rooted or stunned can't move
        {
            return !ChaseToCombact();
        }
        else
        {
            return false;
        }
    }

    // ACTIONS

    public void ChaseBoos()//avvicinati al boss
    {
        if (!GetComponent<BruiserProfile>().isDashing)
        {
            if (!GetComponent<BruiserMovement>().chaseFlag)
            {
                //Debug.Log("ChaseBoos");
                GetComponent<BruiserMovement>().chaseFlag = true;
            }
        }
      

    }


    public void combactFase()
    {
        //Debug.Log("Combact Fase BBRUISER");
        if (GetComponent<BruiserProfile>().publicGetStatus() != 2) //if 2 = stunned can't attack and move
        {
            fsmCombact.Update();
        }
    }




    //////////////////// COMBACT FSM //////////////////////////////

    // CONDITIONS


    public bool AttkToHeal()
    {
        if (!GetComponent<BruiserProfile>().swordActive && ( (GetComponent<BruiserProfile>().lifeUnderSixty() && !GetComponent<BruiserProfile>().cooldownHeal) ||
            ( (!GetComponent<BruiserProfile>().cooldownDash && GetComponent<moreSpecificProfile>().publicGetStatus() == 0) && boss.GetComponent<BossProfile>().isAttacking && (boss.GetComponent<BossProfile>().instanceIDtarget == this.gameObject.GetInstanceID() || (boss.GetComponent<BossProfile>().isUsingAoE)) ) ) )
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
        if (!GetComponent<BruiserProfile>().swordActive && !GetComponent<BruiserProfile>().cooldownSpecial && (!GetComponent<BruiserProfile>().lifeUnderSixty() || GetComponent<BruiserProfile>().cooldownHeal) )
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
        if (!GetComponent<BruiserProfile>().isHealing && GetComponent<BruiserProfile>().cooldownSpecial)
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
        if (!GetComponent<BruiserProfile>().isHealing && !GetComponent<BruiserProfile>().cooldownSpecial)
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
        if ( (!GetComponent<BruiserProfile>().lifeUnderSixty() || GetComponent<BruiserProfile>().cooldownHeal) ||
            ((GetComponent<BruiserProfile>().cooldownDash || GetComponent<moreSpecificProfile>().publicGetStatus() != 0) || !boss.GetComponent<BossProfile>().isAttacking || (boss.GetComponent<BossProfile>().instanceIDtarget != this.gameObject.GetInstanceID() || (!boss.GetComponent<BossProfile>().isUsingAoE))))
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
        if ( (GetComponent<BruiserProfile>().lifeUnderSixty() && !GetComponent<BruiserProfile>().cooldownHeal) ||
            ((!GetComponent<BruiserProfile>().cooldownDash && GetComponent<moreSpecificProfile>().publicGetStatus() == 0) && boss.GetComponent<BossProfile>().isAttacking && (boss.GetComponent<BossProfile>().instanceIDtarget == this.gameObject.GetInstanceID() || (boss.GetComponent<BossProfile>().isUsingAoE))))
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
        if (!firstRush)
        {
            GetComponent<BruiserProfile>().attackWithSword();
        }
        else
        {
            firstRush = false;
        }
    }

    public void Heal()
    {
        //Uso heal in priorita' se disponibile se no dash
        if (GetComponent<BruiserProfile>().lifeUnderSixty() && !GetComponent<BruiserProfile>().cooldownHeal)
        {
            Debug.Log("Uso Heal");
            GetComponent<BruiserProfile>().healHimSelf();
        }
        else
        {
            GetComponent<BruiserProfile>().rollAway();
        }
       
    }

    public void ActiveSpecial()
    {
        GetComponent<BruiserProfile>().activateUlti();
    }
}
