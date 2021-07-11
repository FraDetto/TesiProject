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
    public GameObject shieldOb;

    // private GameObject obstacles;

    private bool shieldActive;
    public float distance;
    private bool firstObj;


    //BISOGNA RIMETTERLI
    void Start()
    {
        //targetBehavior = transform.parent.GetComponentInChildren<BossBehavior>();
        //bossAttackBehav = transform.parent.GetComponent<BossAttackBehavior>();
    }

        //this.OnEpisodeBegin();
        /*this.RequestDecision();
        Academy.Instance.EnvironmentStep();*/



    public override void OnEpisodeBegin()
    {
        //Debug.Log(" =====OnEPISODE BEGIN  MOVING=====  ");
        transform.parent.localPosition = new Vector3(30f, 4.7f, 20f);
        //shieldOb = null;
        //GetComponentInParent<moreSpecificProfile>().setShieldForBoss(0);
        nOfObjShieldSpawned = 0;
        rewardOfEp = 0f;
        bossIsRunning = false;

        shieldActive = false;

        //gameManager.generateShieldObj();
        shieldOb.SetActive(true);
        shieldOb.GetComponent<shieldObjTrigger>().startTimeAbilitation();

        distance = 0f;

        firstObj = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log(" =====CollectObservations MOVING===== " + (transform.parent.localPosition - shieldOb.transform.localPosition).magnitude);

        sensor.AddObservation(transform.parent.localPosition);

        sensor.AddObservation(shieldOb.transform.localPosition);

        sensor.AddObservation((transform.parent.localPosition - shieldOb.transform.localPosition).magnitude);

        sensor.AddObservation(distance);
        /*if (null != shieldOb)
        {
            sensor.AddObservation(shieldOb.transform.position);
        }*/

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        //Debug.Log(" =====OnActionReceived===== " + " MOVING ");

        float moveX = vectorAction[0];
        float moveZ = vectorAction[1];


        float moveSpeed = 20f;

        //if (null != shieldOb)
        if(shieldActive)
        {
           

            if ( ((transform.parent.localPosition - shieldOb.transform.localPosition).magnitude) < distance)
            {
                this.AddReward(+0.05f);
                rewardOfEp += +0.05f;
            }
            else
            {
                this.AddReward(-0.05f);
                rewardOfEp += -0.05f;
            }

            distance = (transform.parent.localPosition - shieldOb.transform.localPosition).magnitude;

            Vector3 verticalAdj = new Vector3(shieldOb.transform.localPosition.x, transform.parent.transform.localPosition.y, shieldOb.transform.localPosition.z);
            transform.parent.transform.LookAt(verticalAdj);

            transform.parent.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;


        }
    }



    public void endEpEdges()
    {
        Debug.Log("BOSS HA HITTATO BORDO  DOVREI FERMARE EPISODIO 1");
        this.AddReward(-0.5f);

        rewardOfEp += -0.5f;
        overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

        /*
        bossAttackBehav.setIsRunning(false);
        targetBehavior.setIsRunning(false);

        bossIsRunning = false;

 
        bossAttackBehav.endEpStopAll();
        targetBehavior.endEpStopAll();
        targetBehavior.setActionTargetNull();
        gameManager.stopRoutManager();
        bossAttackBehav.endEpAttkBe();
        targetBehavior.endHittedObstacle();

        this.EndEpisode();*/
        bossIsRunning = false;
        //gameManager.stopRoutManager();

        this.EndEpisode();

    }

    public void hitObjShield()
    {
        Debug.Log("BOSS HA PRESO SCUDO");
        this.AddReward(+1f);

       
        rewardOfEp += 1f;

        overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
        shieldActive = false;
        bossIsRunning = false;
        this.EndEpisode();

        /*
        if (firstObj)
        {
            overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
            firstObj = false;
            shieldActive = false;
        }
        else
        {
            overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.magenta);

            shieldOb.SetActive(false);

            bossIsRunning = false;
            shieldActive = false;
            this.EndEpisode();
        }*/







        //gameManager.stopRoutManager();

        /*bossAttackBehav.setIsRunning(false);
        targetBehavior.setIsRunning(false);

        bossIsRunning = false;
        */



        /*if (nOfObjShieldSpawned < 2)
        {
            //GetComponentInParent<moreSpecificProfile>().setShieldForBoss(400);
            gameManager.ableRoutineForObstacles();
            
        }
        else
        {/*
            overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);

            gameManager.stopRoutManager();
            this.EndEpisode();
            /*
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
            */
        //}



        /*
        GetComponentInParent<moreSpecificProfile>().setShieldForBoss(400);


        overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
        
        bossAttackBehav.endEpStopAll();
        targetBehavior.endEpStopAll();
        targetBehavior.setActionTargetNull();
        gameManager.stopRoutManager();
        bossAttackBehav.endEpAttkBe();
        targetBehavior.endHittedObstacle();

        this.EndEpisode();*/
    }

    public void hitObstaclesWall()
    {
        Debug.Log("BOSS HA HITTATO OBSTACLES 2");
        this.AddReward(-0.5f);

        rewardOfEp += -0.5f;

    }

    public void setShieldObj(GameObject sobj)
    {

        shieldOb = sobj;

        //bossAttackBehav.setIsRunning(true);

        //targetBehavior.setShieldObj(sobj);
        bossIsRunning = true;
    }

    public void setActiveShieldObj(bool flag)
    {
        shieldActive = flag;
    }

    public void whenEpEnd()//when episode end because it or the party has lose
    {
        this.EndEpisode();
    }

}


