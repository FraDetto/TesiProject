using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class borderForTrainingBossMov : MonoBehaviour
{
    public GameObject boss;

    public void OnTriggerEnter(Collider other)
    {
        if (null != other.GetComponent<moreSpecificProfile>() &&  other.transform.tag.Equals("Boss"))
        {
            Debug.Log("BOSS HA HITTATO BORDO  DOVREI FERMARE EPISODIO 1");
            boss.GetComponentInChildren<BossMovingBehavior>().endEpEdges();
        }
    }
}
