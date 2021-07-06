using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldObjTrigger : MonoBehaviour
{

    public GameObject boss;

    public void startTimeAbilitation()
    {
        gameObject.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
        takePos();
        StartCoroutine(abilitateShieldObj());
    }
    


    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Equals("Boss"))
        {

            Debug.Log("BOSS HA HITTATO SHHIELDOBJ");
            boss.GetComponentInChildren<BossMovingBehavior>().hitObjShield();
            //Destroy(this.gameObject);
            moveThisObj();
        }
    }

    public void setBoss(GameObject b)
    {
        boss = b;
    }

    public IEnumerator abilitateShieldObj()
    {
        yield return new WaitForSeconds(10.0f);

        gameObject.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        moveThisObj();

    }

    public void moveThisObj()
    {
        boss.GetComponentInChildren<BossMovingBehavior>().setActiveShieldObj(true);
    }

    public void takePos()
    {
        float randomX = 0.0f;
        float randomZ = 0.0f;

        randomX = Random.Range(15f, 75.0f);
        randomZ = Random.Range(15.0f, +55.0f);

        this.gameObject.transform.localPosition = new Vector3(randomX,3.83f, randomZ);
    }

}
