using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldObjTrigger : MonoBehaviour
{
    private GameObject boss;

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Equals("Boss"))
        {

            Debug.Log("BOSS HA HITTATO SHHIELDOBJ");
            boss.GetComponentInChildren<BossMovingBehavior>().hitObjShield();
            Destroy(this.gameObject);
        }
    }

    public void setBoss(GameObject b)
    {
        boss = b;
    }
}
