using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMainFSM : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    public float reactionTime = 2.0f;
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
        FSMTransition t3 = new FSMTransition(AttkToDef);//CleaningFSM.Update
        FSMTransition t4 = new FSMTransition(AttkToSpec);
        FSMTransition t5 = new FSMTransition(DefToAttk); // different from t1
        FSMTransition t6 = new FSMTransition(DefToSpec);
        FSMTransition t7 = new FSMTransition(SpecToAttk);


        // Link states with transitions
        Attack.AddTransition(t3, Defend);
        Attack.AddTransition(t4, Special);

        Defend.AddTransition(t5, Attack);
        Defend.AddTransition(t6, Special);

        Special.AddTransition(t7, Defend);

        // Setup a FSA at initial state
        fsmCombact = new FSM(Attack);

        // Start monitoring
        StartCoroutine(Patrol());
    }




    // Periodic update, run forever
    public IEnumerator Patrol()
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
        return true;
    }

    public bool CombactToChase()
    {
        return true;
    }

    // ACTIONS

    public void ChaseBoos()//avvicinati al boss
    {

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
