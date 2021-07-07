using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldObjTrigger : MonoBehaviour
{

    public GameObject boss;

    private bool firstRun;

    public void startTimeAbilitation()
    {
        firstRun = true;
        gameObject.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
        takePosR();
        StartCoroutine(abilitateShieldObj());
    }
    


    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Equals("Boss"))
        {
            Debug.Log("BOSS HA HITTATO SHHIELDOBJ");
            if (firstRun)
            {
                boss.GetComponentInChildren<BossMovingBehavior>().hitObjShield();
                //Destroy(this.gameObject);
                moveThisObj();
                firstRun = false;
            }
            else
            {
                boss.GetComponentInChildren<BossMovingBehavior>().hitObjShield();
                this.gameObject.transform.localPosition = new Vector3(0f, -6f, 0f);
            }

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
        actThisObj();

    }

    public void actThisObj()
    {
        boss.GetComponentInChildren<BossMovingBehavior>().setActiveShieldObj(true);
    }

    public void takePosR()
    {
        float randomX = 0.0f;
        float randomZ = 0.0f;

        randomX = Random.Range(15f, 75.0f);
        randomZ = Random.Range(15.0f, +55.0f);

        this.gameObject.transform.localPosition = new Vector3(randomX,3.83f, randomZ);
    }

    public void takePosL()
    {
        float randomX = 0.0f;
        float randomZ = 0.0f;

        //randomX = Random.Range(-15f, -75.0f);
        //randomZ = Random.Range(15.0f, +55.0f);
        randomX = -65f;
        randomZ = 40f;

        this.gameObject.transform.localPosition = new Vector3(randomX, 3.83f, randomZ);
    }

    public void moveThisObj()
    {
        gameObject.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
        takePosL();
        StartCoroutine(abilitateShieldObj());
    }

}
