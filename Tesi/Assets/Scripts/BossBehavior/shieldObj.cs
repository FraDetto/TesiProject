using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class shieldObj : MonoBehaviour
{
    public Agent bossRef;

    public void setBoss(Agent b)
    {
        bossRef = b;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Boss"))
        {
            other.gameObject.GetComponent<moreSpecificProfile>().shieldBar.setMaxShield(400);
            Destroy(this.transform.parent.gameObject);
        }
    }

}
