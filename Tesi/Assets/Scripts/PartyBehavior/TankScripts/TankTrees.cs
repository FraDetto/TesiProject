using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrees : MonoBehaviour
{
    public float reactionTime = 3f;

    private Rigidbody myRigidbody;
    private DecisionTree dt;

    private void AttackTransitionTree()
    {
        // Define actions
        DTAction a1 = new DTAction(chaseBoos);
        DTAction a2 = new DTAction(attackTheBoss);
        DTAction a3 = new DTAction(defendFromTheBoss);
        DTAction a4 = new DTAction(chooseAttackOrDef);
        DTAction a5 = new DTAction(useSpecial);

        // Define decisions
        DTDecision d1 = new DTDecision(checkConditionsForSpecial);
        DTDecision d2 = new DTDecision(checkBossPosition);
        DTDecision d3 = new DTDecision(checkConditionForNotDef);
        DTDecision d4 = new DTDecision(checkBossUlting);//controllare se scudo up


        // Link action with decisions
        d1.AddLink(false, d2);
        d1.AddLink(true, a5);

        d2.AddLink(true, a1);
        d2.AddLink(false, d3);

        d3.AddLink(true, a2);
        d3.AddLink(false, d4);

        d4.AddLink(true, a3);
        d4.AddLink(false, a4);

        // Setup my DecisionTree at the root node
        dt = new DecisionTree(d1);
        myRigidbody = GetComponent<Rigidbody>();

    }

    //CREATE AND GET METHODS
    public DecisionTree CreateAndGetAttackTransitionTree()
    {
        AttackTransitionTree();
        return dt;
    }

    public DecisionTree getCreateAndGetAttackTransitionTree()
    {
        return dt;
    }



    public object checkConditionsForSpecial(object o)//is time to use the special?
    {
        if (reactionTime <= 0)
        {
            return false;
        }
        return true;
    }

    public object checkBossPosition(object o)//control if the boss is in range
    {
        if (reactionTime <= 0)
        {
            return false;
        }
        return true;
    }

    public object checkConditionForNotDef(object o)//see if is time to attack or to def
    {
        if (reactionTime <= 0)
        {
            return false;
        }
        return true;
    }

    public object checkBossUlting(object o)//is the boss ulting? is the shield up in case?
    {
        if (reactionTime <= 0)
        {
            return false;
        }
        return true;
    }


    public object chaseBoos(object o)
    {

        return null;
    }
    public object attackTheBoss(object o)
    {

        return null;
    }

    public object defendFromTheBoss(object o)
    {

        return null;
    }

    public object chooseAttackOrDef(object o)
    {

        return null;
    }

    public object useSpecial(object o)
    {

        return null;
    }
}
