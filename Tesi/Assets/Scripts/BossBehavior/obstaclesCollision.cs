using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstaclesCollision : MonoBehaviour
{
    public GameObject boss;

    public void setBoss(GameObject b)
    {
        boss = b;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.tag.Equals("Boss"))
        {
            Debug.Log("HITTATO BOSS DOVREI FERMARE EPISODIO");
        }
    }
}
