using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BossMovingBehavior : Agent
{

    public GameManager gameManager;
    public int nOfObjShieldSpawned;
    public GameObject overcomeBattleSign;

    public float rewardOfEp;

    public bool bossIsRunning;

    private BossAttackBehavior bossAttackBehav;
    private BossBehavior targetBehavior;
    private GameObject shieldOb;

   // private GameObject obstacles;
    
    

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
        Debug.Log(" =====OnEPISODE BEGIN  MOVING=====  ");
        transform.parent.position = new Vector3(0, 4.7f, 0);
        shieldOb = null;
        //obstacles = null;
        GetComponentInParent<moreSpecificProfile>().setShieldForBoss(0);
        nOfObjShieldSpawned = 0;
        rewardOfEp = 0f;
        bossIsRunning = false;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log(" =====CollectObservations MOVING===== ");

        sensor.AddObservation(transform.parent.position);

        /*if(null != obstacles)
        {
            sensor.AddObservation(obstacles.transform.position);
        }*/

        if (null != shieldOb)
        {
          sensor.AddObservation(shieldOb.transform.position);
        }

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Debug.Log(" =====OnActionReceived===== " + " MOVING ");

        float moveX = vectorAction[0];
        float moveZ = vectorAction[1];
        float moveSpeed = 20f;

        if (null != shieldOb)
        {
            transform.parent.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;  
        }
    }



    public void endEpEdges()
    {
        Debug.Log("BOSS HA HITTATO BORDO  DOVREI FERMARE EPISODIO 2");
        this.AddReward(-0.5f);

        rewardOfEp += -0.5f;
        overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

        bossAttackBehav.setIsRunning(false);
        targetBehavior.setIsRunning(false);

        bossIsRunning = false;

        //gestione dell'end episode delle altre due brain
        bossAttackBehav.endEpStopAll();
        targetBehavior.endEpStopAll();
        targetBehavior.setActionTargetNull();
        gameManager.stopRoutManager();
        bossAttackBehav.endEpAttkBe();
        targetBehavior.endHittedObstacle();

        this.EndEpisode();
    }

    public void hitObjShield()
    {
        Debug.Log("BOSS HA PRESO SCUDO");
        this.AddReward(+1f);

        rewardOfEp += 1f;
        bossAttackBehav.setIsRunning(false);
        targetBehavior.setIsRunning(false);

        bossIsRunning = false;
        /*
        if (nOfObjShieldSpawned < 2)
        {
            GetComponentInParent<moreSpecificProfile>().setShieldForBoss(400);
            gameManager.ableRoutineForObstacles();
            
        }
        else
        {
            
            if (rewardOfEp == 1)
            {
                overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
            }
            else
            {
                overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            }

            bossAttackBehav.endEpStopAll();
            targetBehavior.endEpStopAll();
            targetBehavior.setActionTargetNull();
            gameManager.stopRoutManager();
            bossAttackBehav.endEpAttkBe();
            targetBehavior.endHittedObstacle();

            this.EndEpisode();
        }*/

        GetComponentInParent<moreSpecificProfile>().setShieldForBoss(400);


        overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);

        bossAttackBehav.endEpStopAll();
        targetBehavior.endEpStopAll();
        targetBehavior.setActionTargetNull();
        gameManager.stopRoutManager();
        bossAttackBehav.endEpAttkBe();
        targetBehavior.endHittedObstacle();

        this.EndEpisode();
    }

    public void hitObstaclesWall()
    {
        Debug.Log("BOSS HA HITTATO OBSTACLES 2");
        this.AddReward(-0.5f);

        rewardOfEp += -0.5f;

    }

    public void setShieldObj(GameObject sobj)
    {
        //obstacles = ob;
        shieldOb = sobj;

        //qua settare nella brain di attacco che sta correndo
        bossAttackBehav.setIsRunning(true);
        //targetBehavior.deRootPlayers();
        targetBehavior.setShieldObj(sobj);
        bossIsRunning = true;
    }

    public void whenEpEnd()//when episode end because it or the party has lose
    {
        this.EndEpisode();
    }

}


