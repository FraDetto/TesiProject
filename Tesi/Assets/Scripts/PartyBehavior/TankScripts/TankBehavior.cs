using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehavior : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    private GameObject boss;
    private Rigidbody myRB;

    public float reactionTime = 1.0f;
    public float distanceRange = 4.0f;
    public float speed = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        ////////// MAIN FSM ///////////////////
        FSMState Chase = new FSMState();
        Chase.enterActions.Add(ChaseBoos);
        Chase.stayActions.Add(ChaseBoos);


        FSMState Combact = new FSMState();
        Combact.enterActions.Add(fsmCombact.Update);
        Combact.stayActions.Add(fsmCombact.Update);

        // Define transitions
        FSMTransition t1 = new FSMTransition(ChaseToCombact);
        FSMTransition t2 = new FSMTransition(CombactToChase);


        // Link states with transitions
        Chase.AddTransition(t1, Combact);
        Combact.AddTransition(t2, Chase);


        // Setup a FSA at initial state
        fsmMain = new FSM(Chase);





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

        boss = GameObject.FindGameObjectWithTag("Boss");
        myRB = GetComponent<Rigidbody>();


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
        if (boss.transform)
        {
            Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
            Vector3 toBossPos = (verticalAdj - transform.position);

            if(toBossPos.magnitude > distanceRange)
            {
                transform.LookAt(verticalAdj);
                myRB.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
            }
        }
    }







    //////////////////// COMBACT FSM //////////////////////////////

    // CONDITIONS


    public bool AttkToDef()
    {
        return true;
    }

    public bool AttkToSpec()
    {
        return true;
    }

    public bool DefToAttk()
    {
        return true;
    }

    public bool DefToSpec()
    {
        return true;
    }

    public bool SpecToAttk()
    {
        return true;
    }

    public bool SpecToDef()
    {
        return true;
    }


    // ACTIONS

    public void AttackBoss()
    {

    }

    public void DefendFromAttack()
    {

    }

    public void ActiveSpecial()
    {

    }
}
