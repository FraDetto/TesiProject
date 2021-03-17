using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserBehavior : MonoBehaviour
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

    public void Heal()
    {

    }

    public void ActiveSpecial()
    {

    }
}
