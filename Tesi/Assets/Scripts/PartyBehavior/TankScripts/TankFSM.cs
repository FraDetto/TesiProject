using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFSM : MonoBehaviour
{
    public float reactionTime = 2.0f;
    private FSM fsm;

    // Start is called before the first frame update
    void Start()
    {
        FSMState Chase = new FSMState();
        Chase.enterActions.Add(ChaseBoos);
        Chase.stayActions.Add(ChaseBoos);


        FSMState Attack = new FSMState();
        Attack.enterActions.Add(AttackBoss);
        Attack.stayActions.Add(AttackBoss);

        FSMState Defend = new FSMState();
        Defend.enterActions.Add(DefendFromAttack);

        FSMState Special = new FSMState();
        Defend.enterActions.Add(ActiveSpecial);

        // Define transitions
        FSMTransition t1 = new FSMTransition(DefToAttk);//CleaningFSM.Update
        FSMTransition t2 = new FSMTransition(DefToSpec);
        FSMTransition t3 = new FSMTransition(DefToChase); // different from t1
        FSMTransition t4 = new FSMTransition(SpecToChase);
        FSMTransition t5 = new FSMTransition(SpecToAttk);

        FSMTransition t6 = new FSMTransition(AttackTreeDecision);

        // Link states with transitions
        Defend.AddTransition(t1, Attack);
        Defend.AddTransition(t2, Special);
        Defend.AddTransition(t3, Chase);

        Special.AddTransition(t4, Chase);
        Special.AddTransition(t5, Attack);

        Attack.AddTransition(t6, Attack);
        Attack.AddTransition(t6, Defend);
        Attack.AddTransition(t6, Special);
        Attack.AddTransition(t6, Chase);

        // Setup a FSA at initial state
        fsm = new FSM(Chase);

        // Start monitoring
        StartCoroutine(Patrol());
    }

    // Periodic update, run forever
    public IEnumerator Patrol()
    {
        while (true)
        {
            fsm.Update();
            yield return new WaitForSeconds(reactionTime);
        }
    }

    // CONDITIONS

    public bool DefToAttk()
    {
        return true;
    }

    public bool DefToSpec()
    {
        return true;
    }

    public bool DefToChase()
    {
        return true;
    }

    public bool SpecToChase()
    {
        return true;
    }

    public bool SpecToAttk()
    {
        return true;
    }

    public bool AttackTreeDecision()
    {
        return true;
    }


    // ACTIONS

    public void AttackBoss()
    {
        
    }

    public void ChaseBoos()//avvicinati al boss
    {
        
    }

    public void DefendFromAttack()
    {
      
    }

    public void ActiveSpecial()
    {

    }


}
