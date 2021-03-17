using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBehavior : MonoBehaviour
{
    private FSM fsmMain;
    private FSM fsmCombact;

    public float reactionTime = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        ////////// MAIN FSM ///////////////////
        FSMState takSafeSpot = new FSMState();
        takSafeSpot.enterActions.Add(takSafeSpotFromBoss);
        takSafeSpot.stayActions.Add(takSafeSpotFromBoss);


        FSMState Combact = new FSMState();
        Combact.enterActions.Add(fsmCombact.Update);
        Combact.stayActions.Add(fsmCombact.Update);

        // Define transitions
        FSMTransition t1 = new FSMTransition(safeSpotToCombact);
        FSMTransition t2 = new FSMTransition(CombactToSafeSpot);


        // Link states with transitions
        takSafeSpot.AddTransition(t1, Combact);
        Combact.AddTransition(t2, takSafeSpot);


        // Setup a FSA at initial state
        fsmMain = new FSM(takSafeSpot);





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
        return true;
    }

    public bool CombactToSafeSpot()
    {
        return true;
    }

    // ACTIONS

    public void takSafeSpotFromBoss()//allontanati dal boss
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
