using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class shieldObj : MonoBehaviour
{
    public Agent bossRef;
    public GameManager gameManager;

    public void setBoss(Agent b)
    {
        bossRef = b;
    }

    public void setGameManager(GameManager gm)
    {
        gameManager = gm;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Boss"))
        {
            other.gameObject.GetComponent<moreSpecificProfile>().setShieldForBoss(400);
            gameManager.ableRoutineForObstacles();
            Destroy(this.transform.parent.gameObject);
        }
    }

}
