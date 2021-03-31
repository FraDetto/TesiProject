using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerBehavior : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    private GameObject boss;
    private Rigidbody myRB;

    public float reactionTime = 2.5f;
    public float distanceRange = 45.0f;
    public bool firstRush = true;

    public float m_HealRadius = 30f;
    public LayerMask m_PlayerMask;
    // Start is called before the first frame update
    void Start()
    {

        boss = GameObject.FindGameObjectWithTag("Boss");
        myRB = GetComponent<Rigidbody>();


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



        // Start monitoring
        StartCoroutine(Fight());
    }




    // Periodic update, run forever
    public IEnumerator Fight()
    {
        while (true)
        {
            fsmMain.Update();
            yield return new WaitForSeconds(reactionTime);
        }
    }




    /////////////////// MAIN FSM ////////////////////////////////

    // CONDITIONS

    public bool safeSpotToCombact()
    {
        if ((boss.transform.position - myRB.transform.position).magnitude >= distanceRange)
        {
            GetComponent<HealerMovement>().chaseFlag = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CombactToSafeSpot()
    {
        return !safeSpotToCombact();
    }
    // ACTIONS

    public void takSafeSpotFromBoss()//allontanati dal boss
    {
        /*Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
        Vector3 toBossPos = (verticalAdj - transform.position);

        if (toBossPos.magnitude <= distanceRange)
        {
            transform.LookAt(verticalAdj);
            myRB.MovePosition(transform.position + (-transform.forward) * speed * Time.deltaTime);
        }*/
        if (!GetComponent<HealerMovement>().chaseFlag)
        {
            GetComponent<HealerMovement>().chaseFlag = true;
        }

    }



    public void combactFase()
    {
        Debug.Log("Combact Fase HEALER");
        fsmCombact.Update();
    }



    //////////////////// COMBACT FSM //////////////////////////////

    // CONDITIONS


    public bool AttkToHeal()
    {
        if (!GetComponent<HealerProfile>().cooldownHeal && !allFullLife() && (GetComponent<HealerProfile>().cooldownSpecial || !boss.GetComponent<BossProfile>().isUsingAoE))
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
        if ( !GetComponent<HealerProfile>().cooldownSpecial && boss.GetComponent<BossProfile>().isUsingAoE)
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
        if ( GetComponent<HealerProfile>().cooldownSpecial || !boss.GetComponent<BossProfile>().isUsingAoE)
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
        if (!GetComponent<HealerProfile>().cooldownSpecial && boss.GetComponent<BossProfile>().isUsingAoE)
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
        if (!firstRush)
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
        GetComponent<HealerProfile>().healAlly();
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

            if (targetProfile.publicGetCurrentLife() != targetProfile.publicGetTotalLife())
            {
                flag = false;
            }

        }
        return flag;
    }
}
