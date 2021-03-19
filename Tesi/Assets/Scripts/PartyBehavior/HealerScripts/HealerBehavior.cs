using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerBehavior : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    private GameObject boss;
    private Rigidbody myRB;

    public float reactionTime = 1.5f;
    public float distanceRange = 15.0f;
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
        return true;
    }

    public bool AttkToSpec()
    {
        return true;
    }

    public bool HealToAttk()
    {
        return true;
    }

    public bool HealToSpec()
    {
        return true;
    }

    public bool SpecToAttk()
    {
        return true;
    }

    public bool SpecToHeal()
    {
        return true;
    }


    // ACTIONS

    public void AttackBoss()
    {

    }

    public void HealAnAlly()
    {

    }

    public void ActiveSpecial()
    {

    }
}
