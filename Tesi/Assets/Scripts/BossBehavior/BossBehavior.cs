using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BossBehavior : Agent
{
    public GameObject[] playersParty;
    public LayerMask m_PlayerMask;
    public GameObject swordSwingAttk;


    public GameManager gameManager;

    public bool firstRun = true;

    public int champsKO;

    public float countRewardRun;

    [SerializeField] private string partyList;
    //public BossProfile myProfile;
    private Vector3 startPosition;

    //private GameObject targetForAttack;
    private int target;


    private int previousTargetID;


    private int[] actionTarget;

    private float bonusFutureReward;



    //TYPECODE PLAYERS 0 TANK, 1 BRUISER, 2 MAGE, 3 HEALER
    Coroutine co;
    Coroutine re;
   
    public GameObject targetPlayer;

    public int instanceIDtarget;
    public Transform swingAttackPosition;

   // private float timeBeforeCastAttracting = 0.4f;

    private BossAttackBehavior attackBehavior;


    public GameObject overcomeBattleSign;
    public GameObject overcomeBattleSignEndRun;
    private GameObject[] endArray;

    /*public bool isAttacking = false;
    public bool isUsingAoE = false;

    public bool isShooting = false;
    public bool isAttracting = false;
    public bool isCastingAoE = false;*/

    // Start is called before the first frame update
    void Start()
    {
        attackBehavior = GetComponentInParent<BossAttackBehavior>();

        Academy.Instance.AutomaticSteppingEnabled = false;

        //this.OnEpisodeBegin();

        this.RequestDecision();
        Academy.Instance.EnvironmentStep();
    }

    public override void OnEpisodeBegin()
    {
        //The episode end when the boss dies or all the party die? I want to do in this way to let the agent understand if an action is ok or not in the long time so ill'give a small rewaerd for an action or a 
        //a sequence of actions that are correct and follow the strategies to defeat the single members of the party and the whole party at the end.
        //
        //At the beginnning of an episode party members are chosen randomly  to enhance the boss's learning
        Debug.Log(" =====OnEPISODE BEGIN  TARGET=====  ");


        //playersParty = gameManager.generatePartyInGame();
        playersParty = gameManager.generateStandardPartyInGame();
        attackBehavior.setParty(playersParty);

        champsKO = 0;
        previousTargetID = 0;
        countRewardRun = 0.0f;
        endArray = playersParty;
        if (!firstRun)
        {          
            //playersParty = gameManager.generateStandardPartyInGame();
            //GetComponentInParent<moreSpecificProfile>().resetBossStats();           
            takeTheAction();            
        }


        


    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //WE do observation on pg in the party
        //WE do a collection of observations throught a bufferSensor that can be useful in situations in which the Agent must pay attention to a varying number of entities.
        //The bufferSensor uses an attention Module: a mechanisms enable solving problems that require comparative reasoning between entities in a scene
        //the BufferSensor can process a variable number of entities, i still need to define a maximum number of entities. This is because it requires to know what the shape of the observations will be.
        //If fewer entities are observed than the maximum, the observation will be padded with zeros and the trainer will ignore the padded observations
        //Cosi quando un pg del party muore lo si puo' disattivare e lui continua con le osservazioni sugli altri?? 
        //The data we will observe are float, int and bool

        //For the best results when training, we should normalize the components of feature vector to the range [-1, +1] or [0, 1]. 
        //When we normalize the values the PPO neural network can often converge to a solution faster.

        Debug.Log(" =====CollectObservations TARGET===== ");

        sensor.AddObservation(previousTargetID);
        sensor.AddObservation(rangedChampAlive());
        sensor.AddObservation(bruiserAlive());

        for (int i=0; i< playersParty.Length; i++)
        {
            sensor.AddObservation(playersParty[i].GetInstanceID());
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().getTypeCode());
        }      
    }




    public override void OnActionReceived(float[] vectorAction)
    {
        //action he will apply are discrete (for the moment):the academy generate an int that corresponds to an action of the boss
        //When using Discrete Actions, it is possible to specify that some actions are impossible for the next decision(i can use it as a sort of cooldown for the boss).
        //Discrete actions can have multiple action branches: i can use 2 branches-> 1 for actions and 1 for targets so it decides which target and which actions do so i can bettere assign rewards

        ////REWARDS////
        ///The PPO reinforcement learning algorithm works by optimizing the choices an agent makes such that the agent earns the highest cumulative reward over time
        ///If there are multiple calls to AddReward() for a single agent decision, the rewards will be summed together to evaluate how good the previous decision was.
        ///Range for rewards between -1 and 1
        ///Th idea is +1 and -1 if boss defeat the party or if it dies respectively

        //In detail for each attack of the boss (ideas)
        
        Debug.Log(" =====OnActionReceived===== " + " TARGET " + vectorAction[0]);// + " AZIONE " + vectorAction[1]);
        /// Number of targets 
        target = Mathf.FloorToInt(vectorAction[0]);

        //targetForAttack = playersParty[target];


        //int actionForBoss = Mathf.FloorToInt(vectorAction[1]);

        //actionChoose[0] = actionForBoss;
        
        //Debug.Log("PLAYER PER PROSSIMO ATTACCO " + playersParty[target].tag + " CON ID "+ playersParty[target].GetInstanceID());

        turnBossToTarget();


        //0 TANK, 1 BRUISER, 2 MAGE, 3 HEALER

        switch (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode())
        {
            case 0:
                if (rangedChampAlive())
                {
                    this.AddReward(-0.5f);
                    countRewardRun += -0.5f;
                }
                else
                {
                    if (bruiserAlive())
                    {
                        this.AddReward(-0.5f);
                        countRewardRun += -0.5f;
                    }
                    else
                    {
                        if (previousTargetID == playersParty[target].GetInstanceID())
                        {
                            this.AddReward(+1f);
                            countRewardRun += 1f;
                        }
                        else if (previousTargetID == 0)
                        {
                            this.AddReward(+1f);
                            countRewardRun += 1f;
                        }
                        else
                        {
                            this.AddReward(-0.5f);
                            countRewardRun += -0.5f;
                        }
                    }
                }
                break;

            case 1:
                if (rangedChampAlive())
                {
                    this.AddReward(-0.5f);
                    countRewardRun += -0.5f;
                }
                else
                {
                    if (previousTargetID == playersParty[target].GetInstanceID())
                    {
                        this.AddReward(+1f);
                        countRewardRun += 1f;
                    }
                    else if (previousTargetID == 0)
                    {
                        this.AddReward(+1f);
                        countRewardRun += 1f;
                    }
                    else
                    {
                        this.AddReward(-0.5f);
                        countRewardRun += -0.5f;
                    }
                }
                break;

            case 2:
                if (previousTargetID == playersParty[target].GetInstanceID())
                {
                    this.AddReward(+1f);
                    countRewardRun += 1f;
                }
                else if(previousTargetID == 0)
                {
                    this.AddReward(+1f);
                    countRewardRun += 1f;
                }
                else
                {
                    this.AddReward(-0.5f);
                    countRewardRun += -0.5f;
                }
                    
                break;

            case 3:
                if (previousTargetID == playersParty[target].GetInstanceID())
                {
                    this.AddReward(+1f);
                    countRewardRun += 1f;
                }
                else if (previousTargetID == 0)
                {
                    this.AddReward(+1f);
                    countRewardRun += 1f;
                }
                else
                {
                    this.AddReward(-0.5f);
                    countRewardRun += -0.5f;
                }
                break;

            default:
                Debug.Log("CHARACTER UNKNOWN");
                break;
        }

        //isAttacking = true;
        targetPlayer = playersParty[target];
        instanceIDtarget = targetPlayer.GetInstanceID();
        previousTargetID = targetPlayer.GetInstanceID();

        attackBehavior.setAllTargetInfo(target);
        //actionForAttack();
        attackBehavior.RequestDecision();
        //Academy.Instance.EnvironmentStep();






        //StartCoroutine(timeBeforeAnOtherAction());


        //this.co = StartCoroutine(timeBeforeAnOtherAction());



    }


    public void actionForTarget()
    {
        this.RequestDecision();
        Academy.Instance.EnvironmentStep();
    }


    public IEnumerator timeBeforeAnOtherAction()
    {
        Debug.Log(" =====DOVREBBE CHIAMARE ALTRA AZIONE TARGET===== ");
        yield return new WaitForSeconds(2.4f);

        this.RequestDecision();
        Academy.Instance.EnvironmentStep();   
    }

    /*
    public IEnumerator timeBeforeDamageTarget()
    {
        //ricordarsi di gestire i cooldown
        //Debug.Log(" =====DANNEGGIO TARGET ===== " + playersParty[target].tag);
        yield return new WaitForSeconds(timeBeforeCastAttracting);
        float damageCharacter = GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
        damageCharacter = ((damageCharacter / 100) * 75);
        playersParty[target].GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(damageCharacter);
    }*/

    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        if (!firstRun)
        {
            if (null != actionTarget)
            {
                Debug.Log(" =====SET MASK TARGET===== " + actionTarget[0]);
                actionMasker.SetMask(0, actionTarget);
            }       
        }
        else
        {
            Debug.Log(" =====SET MASK FIRST RUN TARGET===== " );
            firstRun = false;
        }
       
    }





   

    public void checkChampDieInFight()
    {
        GameObject[] arrayPlayerForAdjustment = removeChampDieInFight();

        adjustPlayerArray(arrayPlayerForAdjustment); // aggiorno array BOSS
    }

    public GameObject[] removeChampDieInFight()
    {
        if (playersParty.Length == 1)
        {
            return null;
        }
        else
        {
            GameObject[] arrNew = new GameObject[playersParty.Length - 1];
            int count = 0;
            //Debug.Log(" LUNGHEZZA PLAYERPARTY TARGET " + playersParty.Length);
            for (int i = 0; i < playersParty.Length; i++)
            {
                if (playersParty[i].GetComponent<moreSpecificProfile>().getStatusLifeChamp() != 1)
                {
                    arrNew[count] = playersParty[i];
                    count++;
                }
            }

            return arrNew;
        }

    }

    public void endEpStopAll()
    {
        StopAllCoroutines();
    }




    public void bossDeath()//BOSS DEAD END EPISODE
    {
        if (GetComponentInParent<moreSpecificProfile>().publicGetCurrentLife() == 0 && GetComponentInParent<moreSpecificProfile>().getStatusLifeChamp() == 1 & champsKO < 4)
        {
            Debug.Log("===== END EPISODE BOSS DEAD =======");

            attackBehavior.endEpStopAll();

            endEpStopAll();
            setActionTargetNull();

            overcomeBattleSignEndRun.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        
            
            if (countRewardRun > 19.5f)
            {
                overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
            }
            else
            {
                overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            }


            foreach (GameObject go in endArray)
            {
                if (!go.transform.tag.Equals("Boss"))
                {
                    go.GetComponent<moreSpecificProfile>().detonation();
                }

            }

            GetComponentInParent<moreSpecificProfile>().setFlaResetEpisode(true);

            attackBehavior.endEpAttkBe();
            //this.AddReward(-1.0f);
            EndEpisode();
        }

    }


    public void partyDeath()
    {
        if (GetComponentInParent<moreSpecificProfile>().publicGetCurrentLife() >= 0 && GetComponentInParent<moreSpecificProfile>().getStatusLifeChamp() == 0)
        {
            Debug.Log("==== PARTY HA PERSO =====");

            attackBehavior.endEpStopAll();

            endEpStopAll();
            setActionTargetNull();

            overcomeBattleSignEndRun.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.green);

            
            if (countRewardRun > 19.5f)
            {
                overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
            }
            else
            {
                overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            }

            foreach (GameObject go in this.endArray)
            {
                if (!go.transform.tag.Equals("Boss"))
                {
                    go.GetComponent<moreSpecificProfile>().detonation();
                }

            }

            GetComponentInParent<moreSpecificProfile>().setFlaResetEpisode(true);

            attackBehavior.endEpAttkBe();
            //this.AddReward(1.0f);
            EndEpisode();
        }
    }






    public bool rangedChampAlive()
    {
        bool flag = false;

        for(int i=0; i<playersParty.Length; i++)
        {
            if(playersParty[i].GetComponent<moreSpecificProfile>().getTypeCode() == 2 || playersParty[i].GetComponent<moreSpecificProfile>().getTypeCode() == 3)//MAGE OR HEALER
            {
                flag = true;
            }
        }

        return flag;
    }

    public bool bruiserAlive()
    {
        bool flag = false;

        for (int i = 0; i < playersParty.Length; i++)
        {
            if (playersParty[i].GetComponent<moreSpecificProfile>().getTypeCode() == 1)//BRUISER
            {
                flag = true;
            }
        }

        return flag;
    }

    public bool targetIsInRange() // true if target in range
    {
        if ((playersParty[target].transform.position - transform.position).magnitude < 10.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



  



    public void adjustPlayerArray(GameObject[] newArrayPlay)/// use that when a player is KO to reduce the number of players in the array    
    {
        champsKO++;
        previousTargetID = 0;
        //if (null == newArrayPlay )//se KO ALL PLAYERS
        if(champsKO == 4)
        {
            //attackBehavior.partyDeath();
            partyDeath();
        }
        else
        {
            playersParty = newArrayPlay;

            attackBehavior.setParty(playersParty);

            int numberOfDiedChamp = 4 - playersParty.Length;

            int[] a = new int[numberOfDiedChamp];

            int targetToMask = 3;
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = targetToMask;
                targetToMask--;
            }
            actionTarget = a;

            for (int i = 0; i < actionTarget.Length; i++)
            {
                Debug.Log( "PLAYER KO ACTION TARGET FORMATO DA " + actionTarget[i]); 
            }
        }
    }

    public void turnBossToTarget()
    {
        Vector3 verticalAdj = new Vector3(playersParty[target].transform.position.x, transform.parent.transform.position.y, playersParty[target].transform.position.z);

        transform.parent.transform.LookAt(verticalAdj);
    }


    public void setActionTargetNull()
    {
        actionTarget = null;
    }

    public int getTarget()
    {
        return target;
    }

    public void takeTheAction()
    {
        this.RequestDecision();
        Academy.Instance.EnvironmentStep();
    }

}
