using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehavior : MonoBehaviour
{
    [Range(0.5f, 1.5f)] public float waitTime = 1f;
    public float reactionTime = 3f;
    private Rigidbody myRigidbody;
    private DecisionTree dt;
    // Start is called before the first frame update
    void Start()
    {
        // Define actions
        DTAction a1 = new DTAction(moveToBoss);
        DTAction a2 = new DTAction(attackTheBoss);
        DTAction a3 = new DTAction(defendFromTheBoss);
        DTAction a4 = new DTAction(useSpecial);

        // Define decisions
        DTDecision d1 = new DTDecision(checkBossPosition);
        DTDecision d2 = new DTDecision(attackOrDefend);
        DTDecision d3 = new DTDecision(timeForSpecial);


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
        StartCoroutine(Fight());
        
    }


    // Take decision every interval, run forever
    public IEnumerator Fight()
    {
        while (true)
        {
            dt.walk();
            yield return new WaitForSeconds(reactionTime);
        }
    }


    public object checkBossPosition(object o)//control if the boss is in range
    {
        if (reactionTime <= 0)
        {
            return false;
        }
        return true;
    }

    public object attackOrDefend(object o)//see if is time to attack or to def
    {
        if (reactionTime <= 0)
        {
            return false;
        }
        return true;
    }

    public object timeForSpecial(object o)//is time to use the special?
    {
        if (reactionTime <= 0)
        {
            return false;
        }
        return true;
    }


    public object moveToBoss(object o)
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

    public object useSpecial(object o)
    {

        return null;
    }
}
