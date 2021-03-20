using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserBehavior : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    private GameObject boss;
    private Rigidbody myRB;

    public float reactionTime = 1.5f;
    public float distanceRange = 6.0f;
    public bool firstRush = true;

    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
        myRB = GetComponent<Rigidbody>();


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

    public bool ChaseToCombact()
    {
        if ((boss.transform.position - myRB.transform.position).magnitude <= distanceRange)
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
        return !ChaseToCombact();
    }

    // ACTIONS

    public void ChaseBoos()//avvicinati al boss
    {

        if (!GetComponent<BruiserMovement>().chaseFlag)
        {
            //Debug.Log("ChaseBoos");
            GetComponent<BruiserMovement>().chaseFlag = true;
        }

    }


    public void combactFase()
    {
        Debug.Log("Combact Fase BBRUISER");
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

    }

    public void ActiveSpecial()
    {

    }
}
