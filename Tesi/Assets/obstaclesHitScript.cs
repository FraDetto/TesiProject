using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstaclesHitScript : MonoBehaviour
{
    private GameObject boss;


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.tag.Equals("Boss"))
        {

            Debug.Log("BOSS HA HITTATO OBSTACLES 1");
            boss.GetComponentInChildren<BossMovingBehavior>().hitObstaclesWall();
        }
    }


    public void setBoss(GameObject b)
    {
        boss = b;
    }
}
