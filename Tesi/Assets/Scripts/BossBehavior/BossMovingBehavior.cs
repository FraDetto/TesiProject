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
            this.AddReward(-0.5f);

            //gestione dell'end episode delle altre due brain
            //bossAttackBehav.endEpStopAll();
            //targetBehavior.endEpStopAll();
            //targetBehavior.setActionTargetNull();
            //gameManager.stopRoutManager();
            //bossAttackBehav.endEpAttkBe();
            //targetBehavior.endHittedObstacle();

            this.EndEpisode();
        }else if (collision.collider.transform.tag.Equals("ShieldPower"))
        {
            this.AddReward(+1f);

            //togliere nella brain di attacco che sta correndo
            //bossAttackBehav.setIsRunning(false);

            GetComponentInParent<moreSpecificProfile>().setShieldForBoss(400);
            gameManager.ableRoutineForObstacles();
            Destroy(collision.collider.gameObject);

        }else if (collision.collider.gameObject.GetComponent<BorderCollisionScript>())
        {
            Debug.Log("BOSS HA HITTATO BORDO  DOVREI FERMARE EPISODIO");
            this.AddReward(-0.8f);

            //gestione dell'end episode delle altre due brain
            //bossAttackBehav.endEpStopAll();
            //targetBehavior.endEpStopAll();
            //targetBehavior.setActionTargetNull();
            //gameManager.stopRoutManager();
            //bossAttackBehav.endEpAttkBe();
            //targetBehavior.endHittedObstacle();

            this.EndEpisode();
        }
    }

    public void setShieldObj(GameObject sobj)
    {
        shieldOb = sobj;
        //qua settare nella brain di attacco che sta correndo
    }

    public void whenEpEnd()//when episode end because it or the party has lose
    {
        this.EndEpisode();
    }

}


