using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrees : MonoBehaviour
{
    public float reactionTime = 3f;

    private Rigidbody myRigidbody;
    private DecisionTree dt;

    void AttackTransitionTree()
    {
        // Define actions
        DTAction a1 = new DTAction(chaseBoos);
        DTAction a2 = new DTAction(attackTheBoss);
        DTAction a3 = new DTAction(defendFromTheBoss);
        DTAction a4 = new DTAction(chooseAttackOrDef);

        // Define decisions
        DTDecision d1 = new DTDecision(checkBossPosition);
        DTDecision d2 = new DTDecision(checkConditionForDef);
        DTDecision d3 = new DTDecision(checkBossUlting);


        // Link action with decisions
        d1.AddLink(false, d2);
        d1.AddLink(true, a1);

        d2.AddLink(true, a2);
        d2.AddLink(false, d3);

        d3.AddLink(true, a3);
        d3.AddLink(false, a4);

        // Setup my DecisionTree at the root node
        dt = new DecisionTree(d1);
        myRigidbody = GetComponent<Rigidbody>();

    }

    public DecisionTree getAttackTransitionTree()
    {
        AttackTransitionTree();
        return dt;
    }

    public object checkBossPosition(object o)//control if the boss is in range
    {
        if (reactionTime <= 0)
        {
            return false;
        }
        return true;
    }

    public object checkConditionForDef(object o)//see if is time to attack or to def
    {
        if (reactionTime <= 0)
        {
            return false;
        }
        return true;
    }

    public object checkBossUlting(object o)//is time to use the special?
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
}
