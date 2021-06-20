using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BossMovingBehavior : Agent
{

    public GameManager gameManager;
    private BossAttackBehavior bossAttackBehav;
    private BossBehavior targetBehavior;
    private GameObject shieldOb;

    void Start()
    {
        targetBehavior = transform.parent.GetComponentInChildren<BossBehavior>();
        bossAttackBehav = transform.parent.GetComponent<BossAttackBehavior>();
    }

        //this.OnEpisodeBegin();
        /*this.RequestDecision();
        Academy.Instance.EnvironmentStep();*/



    public override void OnEpisodeBegin()
    {
        transform.parent.position = new Vector3(0, 4.7f, 0);
        shieldOb = null;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);

        if (null != shieldOb)
        {
          sensor.AddObservation(shieldOb.transform.position);
        }

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float moveX = vectorAction[0];
        float moveZ = vectorAction[1];

        if (null != shieldOb)
        {
            transform.parent.position += new Vector3(moveX, transform.parent.position.y, moveZ);  
        }
    }

    private void OnCollisionEnter(Collision collision)//when boss touched the obstacles and not the shield obj
    {
        if (collision.collider.transform.tag.Equals("Obstacles"))
        {
            Debug.Log("BOSS HA HITTATO OBSTACLES  DOVREI FERMARE EPISODIO");
           
        }
    }


    public void whenEpEnd()//when episode end because it or the party has lose
    {

    }

}


