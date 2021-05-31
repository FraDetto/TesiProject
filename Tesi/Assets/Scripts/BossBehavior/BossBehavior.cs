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

    public GameObject overcomeBattleSign;
    public GameObject overcomeBattleSignEndRun;

    public GameManager gameManager;

    public bool firstRun = true;

    public int champsKO;

    public float countRewardRun;

    [SerializeField] private string partyList;
    //public BossProfile myProfile;
    private Vector3 startPosition;

    //private GameObject targetForAttack;
    private int target;

    private bool chainRanged = false;
    private bool chainRay = false;

    private bool breakBefore = false;

    private int previousTargetID;

    private int[] actionChoose = new int[1];
    private int[] actionTarget;

    private float bonusFutureReward;

    private bool m_HitDetect_swing_right;
    private bool m_HitDetect_swing_left;

    RaycastHit m_Hit_swing_right;
    RaycastHit m_Hit_swing_left;


    //TYPECODE PLAYERS 0 TANK, 1 BRUISER, 2 MAGE, 3 HEALER
    Coroutine co;
    Coroutine re;
    private GameObject[] endArray;

    //public GameObject[] playersParty;
    public float speedRangedAttk = 35.0f;

    public int codeAttack = 0; // 0:swing  1: ahead  2:break    i use it to divide the different attack and apply the correct damage

    public GameObject goRangedAttk;
    //public GameObject swordSwingAttk;
    public GameObject swordAheadAttk;
    public GameObject containerAoEAttk;

    //public LayerMask m_PlayerMask;

    public Rigidbody rb;
    public GameObject targetPlayer;

    public GameObject targetPlayerForRay;


    public int instanceIDtarget;
    //public string target;
    private GameObject goBreak;
    private GameObject goRanged;
    private GameObject goSwing;
    private GameObject goAhead;
    private GameObject goAoE;

    public Transform rangedAttackPosition;
    public Transform swingAttackPosition;
    public Transform aheadAttackPosition;//stessa posizione usata per il break
    public Transform AoEAttackPosition;
    //public GameManager gameManager;

    private Vector3 scaleChange = new Vector3(0.16f, 0.018f, 0.16f);
    private Vector3 originalPositionTarget;

    private float velocityAttraction = 26.0f;

    public bool isAttacking = false;
    public bool isUsingAoE = false;

    public bool isShooting = false;
    public bool isAttracting = false;
    public bool isCastingAoE = false;

    private float AoEDuration = 1.2f;
    private float attractingRootDuration = 2.0f;
    private float timeBeforeCastAttracting = 0.4f;


    private GameObject reserveVarTarget;
    private int reserveVarIDtarget;





    // Start is called before the first frame update
    void Start()
    {
        //myProfile = GetComponent<BossProfile>();
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
        //Debug.Log(" =====OnEPISODE BEGIN===== ");
        

        if (!firstRun)
        {
           playersParty = gameManager.generatePartyInGame();
            //playersParty = gameManager.generateStandardPartyInGame();
            GetComponent<moreSpecificProfile>().resetBossStats();
            takeTheAction();
            
        }
        else
        {
            playersParty = gameManager.generatePartyInGame();
            //playersParty = gameManager.generateStandardPartyInGame();
            //firstRun = false;
        }

        champsKO = 0;
        previousTargetID = 0;
        countRewardRun = 0.0f;

        endArray = playersParty;

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

        ///Debug.Log(" =====CollectObservations===== ");


        //sensor.AddObservation(transform.position);
        //sensor.AddObservation(GetComponent<moreSpecificProfile>().publicGetCurrentLife());
        //sensor.AddObservation(GetComponent<moreSpecificProfile>().getStatusLifeChamp());
        //sensor.AddObservation(champsKO);
        //sensor.AddObservation(chainRanged);
        //sensor.AddObservation(chainRay);
        sensor.AddObservation(previousTargetID);
        //sensor.AddObservation(targetInAoErange());
        sensor.AddObservation(rangedChampAlive());
        sensor.AddObservation(bruiserAlive());

        for (int i=0; i< playersParty.Length; i++)
        {
            //sensor.AddObservation(playersParty[i].transform.position);
            //sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().publicGetCurrentLife());
            //sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().getStatusLifeChamp());
            sensor.AddObservation(playersParty[i].GetInstanceID());
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().getTypeCode());
        }

        
        
    }

    /*
    public override void Heuristic(float[] actionsOut)
    {
        
        actionsOut[0] = tOne;
        target = Mathf.FloorToInt(actionsOut[0]);
        Debug.Log("  TONE  " + actionsOut[0]);

    }*/


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

        //myProfile.hubAttacks(actionForBoss, playersParty[target]);

        //0 TANK, 1 BRUISER, 2 MAGE, 3 HEALER
        /*if (playersParty[target].GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 1)
        {
            this.AddReward(-1f);
            countRewardRun += -1f;
        }
        else
        {*/
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
                        if (previousTargetID == targetPlayer.GetInstanceID())
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
                    if (previousTargetID == targetPlayer.GetInstanceID())
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
                if (previousTargetID == targetPlayer.GetInstanceID())
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
                if (previousTargetID == targetPlayer.GetInstanceID())
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

        isAttacking = true;
        targetPlayer = playersParty[target];
        instanceIDtarget = targetPlayer.GetInstanceID();
        previousTargetID = targetPlayer.GetInstanceID();
        StartCoroutine(timeBeforeDamageTarget());
    
        


        StartCoroutine(timeBeforeAnOtherAction());
        //this.co = StartCoroutine(timeBeforeAnOtherAction());

        /*
        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            isAttacking = true;

            Debug.Log("PLAYER PER PROSSIMO ATTACCO " + player.tag + " ID " + player.GetInstanceID());
            reserveVarIDtarget = player.GetInstanceID();
            reserveVarTarget = player;
            //Debug.Log("PLAYER RISERVA " + reserveVarTarget.tag + " ID " + reserveVarIDtarget);

            switch (attackCode)
            {
                case 0://RANGED 
                    targetPlayer = player;
                    instanceIDtarget = targetPlayer.GetInstanceID();

                    StartCoroutine(timeBeforeCastRangedAttack());
                    break;
                case 1://RAY
                    targetPlayerForRay = player;
                    instanceIDtarget = targetPlayerForRay.GetInstanceID();
                    Debug.Log("SONO IN RAY IN HUB " + targetPlayerForRay + " ID CON " + instanceIDtarget);
                    StartCoroutine(timeBeforeCastRayAttack());
                    break;
                case 2://SWING
                    targetPlayer = player;
                    instanceIDtarget = targetPlayer.GetInstanceID();
                    isUsingAoE = true;

                    StartCoroutine(timeBeforeCastSwingAttk());
                    break;
                case 3://AHEAD
                    targetPlayer = player;
                    instanceIDtarget = targetPlayer.GetInstanceID();

                    StartCoroutine(timeBeforeCastAheadAttk());
                    break;
                case 4://BREAK
                    targetPlayer = player;
                    instanceIDtarget = targetPlayer.GetInstanceID();
                    Debug.Log("SONO IN BREAK IN HUB " + targetPlayer + " ID CON " + instanceIDtarget);
                    StartCoroutine(timeBeforeCastBreakAttk());
                    break;
                default://AoE
                    isUsingAoE = true;

                    StartCoroutine(timeBeforeCastAoEAttk());
                    break;
            }
        }
        else
        {
            Debug.Log("SONO IN HUB MA EP FINITPO " + GetComponent<moreSpecificProfile>().flagResetepisode);

        }
        */









































        /*

        Debug.Log(" =====VALUE REWARD===== " + actionChoose[0]);
        if (chainRanged)
        {
            if (!chainRay)
            {
                if (actionForBoss == 1)//RAY ATTACK
                {
                    if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 2 && playersParty[target].GetInstanceID() == previousRangedTargetID)//MAGE
                    {
                        chainRay = true;
                        this.AddReward(0.15f);
                        //qua mettere il fatto che non puo' usare stessa azione dopo                     
                    }
                    else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 3 && playersParty[target].GetInstanceID() == previousRangedTargetID)//HEALER
                    {
                        chainRay = true;
                        //qua mettere il fatto che non puo' usare stessa azione dopo
                        this.AddReward(0.15f);
                    }
                    else
                    {
                        chainRanged = false;
                        previousRangedTargetID = 0;
                        this.AddReward(-0.15f);
                    }
                }
                else
                {
                    chainRanged = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.15f);
                }


            }
            else
            {
                if (actionForBoss == 0)//RANGED ATTACK
                {
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.15f);
                }
                else if (actionForBoss == 1)//RAY
                {
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.15f);
                }
                else if (actionForBoss == 2)//SWING ATTACK
                {
                    if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 2 && playersParty[target].GetInstanceID() == previousRangedTargetID)//MAGE
                    {
                        if (swingRayCastControll())
                        {
                            this.AddReward(0.12f + bonusFutureReward);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        else
                        {
                            this.AddReward(-0.21f);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }

                    }
                    else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 3 && playersParty[target].GetInstanceID() == previousRangedTargetID)//HEALER
                    {
                        if (swingRayCastControll())
                        {
                            this.AddReward(0.12f + bonusFutureReward);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        else
                        {
                            this.AddReward(-0.12f);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                    }
                    else
                    {
                        this.AddReward(-0.15f);
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                }
                else if (actionForBoss == 3)//AHEAD ATTACK
                {
                    if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 2 && playersParty[target].GetInstanceID() == previousRangedTargetID)//MAGE
                    {
                        if (!swingRayCastControll())
                        {
                            this.AddReward(0.12f + bonusFutureReward);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        else
                        {
                            this.AddReward(-0.12f);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                    }
                    else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 3 && playersParty[target].GetInstanceID() == previousRangedTargetID)//HEALER
                    {
                        if (!swingRayCastControll())
                        {
                            this.AddReward(0.12f + bonusFutureReward);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        else
                        {
                            this.AddReward(-0.12f);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                    }
                    else
                    {
                        this.AddReward(-0.2f);
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }


                }
                else if (actionForBoss == 4)//BREAK ATTACK
                {
                    if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 2 && playersParty[target].GetInstanceID() == previousRangedTargetID)//MAGE
                    {
                        this.AddReward(0.08f + bonusFutureReward);
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 3 && playersParty[target].GetInstanceID() == previousRangedTargetID)//HEALER
                    {
                        this.AddReward(0.08f + bonusFutureReward);
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else
                    {
                        this.AddReward(-0.15f);
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                }
                else// ACTION 5 AoE
                {

                    this.AddReward(-0.15f);
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
            }


        }
        else if (chainRay)
        {
            if (actionForBoss == 0)//RANGED ATTACK
            {
                this.AddReward(-0.18f);
                chainRay = false;
                bonusFutureReward = 0.0f;
                previousRangedTargetID = 0;
            }
            else if (actionForBoss == 2)//SWING ATTACK
            {
                if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 2 && playersParty[target].GetInstanceID() == previousRangedTargetID)//MAGE
                {
                    if (swingRayCastControll())
                    {
                        this.AddReward(0.12f + bonusFutureReward);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else
                    {
                        this.AddReward(-0.08f);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }

                }
                else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 3 && playersParty[target].GetInstanceID() == previousRangedTargetID)//HEALER
                {
                    if (swingRayCastControll())
                    {
                        this.AddReward(0.12f + bonusFutureReward);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else
                    {
                        this.AddReward(-0.08f);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                }
                else
                {
                    this.AddReward(-0.2f);
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
            }
            else if (actionForBoss == 3)//AHEAD ATTACK
            {
                if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 2 && playersParty[target].GetInstanceID() == previousRangedTargetID)//MAGE
                {
                    if (!swingRayCastControll())
                    {
                        this.AddReward(0.1f + bonusFutureReward);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else
                    {
                        this.AddReward(-0.08f);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                }
                else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 3 && playersParty[target].GetInstanceID() == previousRangedTargetID)//HEALER
                {
                    if (!swingRayCastControll())
                    {
                        this.AddReward(0.1f + bonusFutureReward);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else
                    {
                        this.AddReward(-0.08f);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                }
                else
                {
                    this.AddReward(-0.2f);
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }

            }
            else if (actionForBoss == 4)//BREAK ATTACK
            {
                if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 2 && playersParty[target].GetInstanceID() == previousRangedTargetID)//MAGE
                {
                    this.AddReward(0.06f + bonusFutureReward);
                    chainRay = false;
                    breakBefore = true;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
                else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 3 && playersParty[target].GetInstanceID() == previousRangedTargetID)//HEALER
                {
                    this.AddReward(0.06f + bonusFutureReward);
                    chainRay = false;
                    breakBefore = true;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
                else
                {
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                    this.AddReward(-0.15f);
                }
            }
            else /// ATTACK 5 AoE
            {

                this.AddReward(-0.15f);
                chainRay = false;
                previousRangedTargetID = 0;
                bonusFutureReward = 0.0f;
            }
        }
        else
        {
            if (actionForBoss == 0)// RANGED ATTACK
            {
                //Debug.Log("FACCIO CONTROLLO METODI RANGED ALIVE " + rangedChampAlive() + " NUMERO ATTORONO " + targetInAoErange());

                if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 2)//MAGE
                {
                    this.AddReward(0.1f);
                    previousRangedTargetID = playersParty[target].GetInstanceID();
                    chainRanged = true;
                    bonusFutureReward = 0.04f;
                }
                else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 3)//HEALER
                {
                    this.AddReward(0.1f);
                    chainRanged = true;
                    previousRangedTargetID = playersParty[target].GetInstanceID();
                    bonusFutureReward = 0.04f;
                }
                else
                {
                    this.AddReward(-0.15f);//neg reward: ranged attack on Bruiser or Tank
                }
                //qua mettere il fatto che non puo' usare stessa azione dopo
            }
            else if (actionForBoss == 1)//RAY ATTACK
            {
                if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 2)//MAGE
                {
                    this.AddReward(0.12f);
                    chainRay = true;
                    bonusFutureReward = 0.06f;
                    previousRangedTargetID = playersParty[target].GetInstanceID();
                }
                else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 3)//HEALER
                {
                    this.AddReward(0.12f);
                    chainRay = true;
                    bonusFutureReward = 0.06f;
                    previousRangedTargetID = playersParty[target].GetInstanceID();
                }
                else
                {
                    this.AddReward(-0.15f);//neg reward: ranged attack on Bruiser or Tank
                    previousRangedTargetID = 0;
                }
                //qua mettere il fatto che non puo' usare stessa azione dopo
            }
            else if (actionForBoss == 2)//SWING ATTACK
            {
                if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 1 || playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 0)//BRUISER OR TANK
                {
                    if (rangedChampAlive())
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.15f);
                    }
                    else
                    {
                        if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 1)//BRUISER
                        {
                            if (playersParty[target].GetInstanceID() == previousRangedTargetID)
                            {
                                if (breakBefore)// attacked Bruiser with a break Attack before
                                {
                                    if (swingRayCastControll())
                                    {
                                        this.AddReward(0.15f);
                                    }
                                    else
                                    {
                                        this.AddReward(-0.08f);
                                    }
                                }
                                else// attacked Bruiser with a NON break Attack before
                                {
                                    if (swingRayCastControll())
                                    {
                                        this.AddReward(0.1f);
                                    }
                                    else
                                    {
                                        this.AddReward(-0.08f);
                                    }
                                }
                                breakBefore = false;
                            }
                            else
                            {
                                if (previousRangedTargetID == 0)
                                {
                                    if (swingRayCastControll())
                                    {
                                        previousRangedTargetID = playersParty[target].GetInstanceID();
                                        this.AddReward(0.1f);
                                    }
                                    else
                                    {
                                        previousRangedTargetID = playersParty[target].GetInstanceID();
                                        this.AddReward(-0.08f);
                                    }
                                }
                                else
                                {
                                    breakBefore = false;
                                    previousRangedTargetID = 0;
                                    this.AddReward(-0.15f);
                                }
                            }

                        }
                        else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 0)//TANK
                        {
                            if (bruiserAlive())
                            {
                                breakBefore = false;
                                previousRangedTargetID = 0;
                                this.AddReward(-0.15f);
                            }
                            else
                            {
                                if (playersParty[target].GetInstanceID() == previousRangedTargetID)
                                {
                                    if (breakBefore)
                                    {
                                        if (swingRayCastControll())
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.15f);
                                        }
                                        else
                                        {
                                            breakBefore = false;
                                            this.AddReward(-0.08f);
                                        }
                                    }
                                    else
                                    {
                                        if (swingRayCastControll())
                                        {
                                            this.AddReward(0.1f);
                                        }
                                        else
                                        {
                                            this.AddReward(-0.08f);
                                        }
                                    }
                                }
                                else
                                {
                                    if (previousRangedTargetID == 0)
                                    {
                                        if (swingRayCastControll())
                                        {
                                            previousRangedTargetID = playersParty[target].GetInstanceID();
                                            this.AddReward(0.1f);
                                        }
                                        else
                                        {
                                            previousRangedTargetID = playersParty[target].GetInstanceID();
                                            this.AddReward(-0.08f);
                                        }
                                    }
                                    else
                                    {
                                        breakBefore = false;
                                        previousRangedTargetID = 0;
                                        this.AddReward(-0.15f);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //attack's target: MAGE or HEALER and obv not in range because is a single attack not after a RAY
                    breakBefore = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.15f);
                }
            }
            else if (actionForBoss == 3)//AHEAD ATTACK
            {
                if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 1 || playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 0)//BRUISER OR TANK
                {
                    if (rangedChampAlive())
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.15f);
                    }
                    else
                    {
                        if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 1)//BRUISER
                        {
                            if (playersParty[target].GetInstanceID() == previousRangedTargetID)
                            {
                                if (breakBefore)// attacked Bruiser with a break Attack before
                                {
                                    if (!swingRayCastControll())
                                    {
                                        breakBefore = false;
                                        this.AddReward(0.15f);
                                    }
                                    else
                                    {
                                        if (playersParty[target].GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (playersParty[target].GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.15f);
                                        }
                                        else
                                        {
                                            breakBefore = false;
                                            this.AddReward(-0.08f);
                                        }

                                    }
                                }
                                else// attacked Bruiser with a NON break Attack before
                                {
                                    if (!swingRayCastControll())
                                    {
                                        this.AddReward(0.1f);
                                    }
                                    else
                                    {
                                        if (playersParty[target].GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (playersParty[target].GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
                                        {
                                            this.AddReward(0.1f);
                                        }
                                        else
                                        {
                                            this.AddReward(-0.08f);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (previousRangedTargetID == 0)
                                {
                                    if (!swingRayCastControll())
                                    {
                                        breakBefore = false;
                                        this.AddReward(0.1f);
                                    }
                                    else
                                    {
                                        if (playersParty[target].GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (playersParty[target].GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.1f);
                                        }
                                        else
                                        {
                                            breakBefore = false;
                                            this.AddReward(-0.08f);
                                        }

                                    }
                                }
                                else
                                {
                                    breakBefore = false;
                                    previousRangedTargetID = 0;
                                    this.AddReward(-0.15f);
                                }
                            }

                        }
                        else if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 0)//TANK
                        {
                            if (bruiserAlive())
                            {
                                breakBefore = false;
                                previousRangedTargetID = 0;
                                this.AddReward(-0.15f);
                            }
                            else
                            {
                                if (playersParty[target].GetInstanceID() == previousRangedTargetID)
                                {
                                    if (breakBefore)
                                    {
                                        if (!swingRayCastControll())
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.15f);
                                        }
                                        else
                                        {
                                            if (playersParty[target].GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (playersParty[target].GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
                                            {
                                                breakBefore = false;
                                                this.AddReward(0.15f);
                                            }
                                            else
                                            {
                                                breakBefore = false;
                                                this.AddReward(-0.08f);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (!swingRayCastControll())
                                        {
                                            this.AddReward(0.1f);
                                        }
                                        else
                                        {
                                            if (playersParty[target].GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (playersParty[target].GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
                                            {
                                                this.AddReward(0.1f);
                                            }
                                            else
                                            {
                                                this.AddReward(-0.08f);
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    if (previousRangedTargetID == 0)
                                    {
                                        if (!swingRayCastControll())
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.1f);
                                        }
                                        else
                                        {
                                            if (playersParty[target].GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (playersParty[target].GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
                                            {
                                                breakBefore = false;
                                                this.AddReward(0.1f);
                                            }
                                            else
                                            {
                                                breakBefore = false;
                                                this.AddReward(-0.08f);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        breakBefore = false;
                                        previousRangedTargetID = 0;
                                        this.AddReward(-0.15f);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //attack's target: MAGE or HEALER and obv not in range because is a single attack not after a RAY
                    previousRangedTargetID = 0;
                    breakBefore = false;
                    this.AddReward(-0.2f);
                }
            }
            else if (actionForBoss == 4)//BREAK ATTACK  ricordare di settare correttamente i flag di break a false anche in ranged e ray sopra quando fa casino
            {
                if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 1 || playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 0)//BRUISER OR TANK
                {
                    if (rangedChampAlive())
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.15f);
                    }
                    else
                    {
                        if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 1)//BRUISER
                        {
                            if (previousRangedTargetID == 0)
                            {
                                breakBefore = true;
                                previousRangedTargetID = playersParty[target].GetInstanceID();
                                this.AddReward(0.07f);
                            }
                            else if (playersParty[target].GetInstanceID() == previousRangedTargetID)
                            {
                                if (!breakBefore)
                                {
                                    breakBefore = true;
                                    this.AddReward(0.08f);
                                }
                                else
                                {
                                    breakBefore = false;
                                    this.AddReward(-0.08f);
                                }
                            }
                            else
                            {
                                previousRangedTargetID = 0;
                                breakBefore = false;
                                this.AddReward(-0.15f);
                            }
                        }
                        else // case of tank
                        {
                            if (playersParty[target].GetComponent<moreSpecificProfile>().getTypeCode() == 0)//TANK
                            {
                                if (bruiserAlive())
                                {
                                    breakBefore = false;
                                    previousRangedTargetID = 0;
                                    this.AddReward(-0.15f);
                                }
                                else
                                {
                                    if (previousRangedTargetID == 0)
                                    {
                                        breakBefore = true;
                                        previousRangedTargetID = playersParty[target].GetInstanceID();
                                        this.AddReward(0.07f);
                                    }
                                    else if (playersParty[target].GetInstanceID() == previousRangedTargetID)
                                    {
                                        if (!breakBefore)
                                        {
                                            breakBefore = true;
                                            this.AddReward(0.08f);
                                        }
                                        else
                                        {
                                            breakBefore = false;
                                            this.AddReward(-0.08f);
                                        }
                                    }
                                    else
                                    {
                                        previousRangedTargetID = 0;
                                        breakBefore = false;
                                        this.AddReward(-0.15f);
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    //attack's target: MAGE or HEALER and obv not in range because is a single attack not after a RAY
                    previousRangedTargetID = 0;
                    breakBefore = false;
                    this.AddReward(-0.15f);
                }
            }
            else // ACTION 5 AoE attack
            {
                if (rangedChampAlive())
                {
                    previousRangedTargetID = 0;
                    breakBefore = false;
                    this.AddReward(-0.15f);
                }
                else
                {
                    if (breakBefore)
                    {
                        if (targetInAoErange() >= 3)
                        {
                            this.AddReward(0.15f);
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            breakBefore = false;
                        }
                        else if (targetInAoErange() >= 1 && targetInAoErange() <= 2)
                        {
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            this.AddReward(-0.08f);
                            breakBefore = false;
                        }
                        else
                        {
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            this.AddReward(-0.15f);
                            breakBefore = false;
                        }
                        //next action can't be that
                    }
                    else
                    {
                        if (targetInAoErange() >= 3)
                        {
                            this.AddReward(0.1f);
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        else if (targetInAoErange() >= 1 && targetInAoErange() <= 2)
                        {
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            this.AddReward(-0.08f);
                        }
                        else
                        {
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            this.AddReward(-0.15f);
                        }
                    }
                }
            }

        }*/



    }
    /*
    public IEnumerator timeValueReward(int actionForBoss)
    {
        //ricordarsi di gestire i cooldown
        yield return new WaitForSeconds(0.5f);
        valueAndApplyReward(actionForBoss);
    }*/

    public IEnumerator timeBeforeAnOtherAction()
    {
        //Debug.Log(" =====DOVREBBE CHIAMARE ALTRA AZIONE===== ");
        yield return new WaitForSeconds(2.4f);

        this.RequestDecision();
        Academy.Instance.EnvironmentStep();   
    }


    public IEnumerator timeBeforeDamageTarget()
    {
        //ricordarsi di gestire i cooldown
        //Debug.Log(" =====DANNEGGIO TARGET ===== " + playersParty[target].tag);
        yield return new WaitForSeconds(timeBeforeCastAttracting);
        float damageCharacter = GetComponent<moreSpecificProfile>().publicGetDamageValue();
        damageCharacter = ((damageCharacter / 100) * 75);
        playersParty[target].GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(damageCharacter);
    }
    
    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        if (!firstRun)
        {
            //Debug.Log(" =====SET MASK ACTION ===== " + actionChoose[0]);
            if (null != actionTarget)
            {
                Debug.Log(" =====SET MASK TARGET===== " + actionTarget[0]);
                actionMasker.SetMask(0, actionTarget);
            }       
            //actionMasker.SetMask(1, actionChoose);
        }
        else
        {
            Debug.Log(" =====SET MASK FIRST RUN===== " );
            firstRun = false;
        }
       
    }


    private void FixedUpdate()
    {
        /*
        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            if (isShooting)
            {
                goRanged.transform.position += goRanged.transform.forward * speedRangedAttk * Time.deltaTime;

                if ((goRanged.transform.position - transform.position).magnitude > (originalPositionTarget - transform.position).magnitude)
                {
                    isAttacking = false;
                    isShooting = false;
                    Destroy(goRanged.gameObject);
                }
            }


            if (isAttracting)
            {

                if ((transform.position - targetPlayerForRay.GetComponent<Rigidbody>().transform.position).magnitude > 8.0f)
                {

                    targetPlayerForRay.GetComponent<Rigidbody>().MovePosition(targetPlayerForRay.transform.position + (targetPlayerForRay.transform.forward) * velocityAttraction * Time.deltaTime);
                }
                else
                {
                    isAttracting = false;
                    isAttacking = false;
                    targetPlayerForRay.GetComponent<moreSpecificProfile>().publicAddRootStatus(attractingRootDuration);//root player

                    targetPlayerForRay = null;
                }
                if (null != targetPlayerForRay)
                {
                    if (targetPlayerForRay.tag.Equals("Tank"))
                    {

                        if (targetPlayerForRay.transform.GetComponent<TankProfile>().shieldActive)
                        {
                            isAttracting = false;

                            isAttacking = false;
                            targetPlayerForRay.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(0);

                            targetPlayerForRay = null;

                        }
                    }
                    else if (targetPlayerForRay.tag.Equals("Mage"))
                    {

                        if (targetPlayerForRay.transform.GetComponent<MageProfile>().defenseActive)
                        {
                            isAttracting = false;

                            isAttacking = false;
                            targetPlayerForRay.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(0);

                            targetPlayerForRay = null;

                            Debug.Log("FALLITO RAY STA DIFENDENDO");
                        }

                    }
                }


            }

            if (isCastingAoE)
            {
                goAoE.transform.localScale += scaleChange * Time.deltaTime;
            }
        }
        else
        {
            if (null != goBreak)
            {
                isShooting = false;
                isAttracting = false;
                Destroy(goBreak.gameObject);
            }
            if (null != goAoE)
            {
                isAttracting = false;
                isCastingAoE = false;
                Destroy(goAoE.gameObject);
            }
            if (null != goRanged)
            {
                isShooting = false;

                Destroy(goRanged.gameObject);
            }
            if (null != goAhead)
            {
                Destroy(goAhead.gameObject);
            }
            isAttacking = false;
            targetPlayer = null;
        }
        */
    }




    public IEnumerator timeBeforeCastRangedAttack()
    {
        //ricordarsi di gestire i cooldown
        originalPositionTarget = targetPlayer.transform.position;
        yield return new WaitForSeconds(timeBeforeCastAttracting);
        removePreviousGo();

        rangedAttack();


    }


    public void rangedAttack()
    {
        //target gia' scelto? o lo sceglie qua?se facessi due brain una per target una per azioni vere e do' reward combinato?
        //Debug.Log(" RANGED BOSS" + targetPlayer.transform.position);
        if (null != goRanged)
        {
            Destroy(goRanged.gameObject);
        }

        turnBossToTargetForRanged();
        goRanged = Instantiate(goRangedAttk, rangedAttackPosition.position, transform.rotation, gameObject.transform);
        isShooting = true;
        goRanged.transform.LookAt(new Vector3(originalPositionTarget.x, 2.0f, originalPositionTarget.z));
    }







    public IEnumerator timeBeforeCastRayAttack()
    {
        //ricordarsi di gestire i cooldown
        yield return new WaitForSeconds(timeBeforeCastAttracting);

        removePreviousGo();
        LaunchRay();


    }

    public void LaunchRay()
    {

        if (targetPlayerForRay == null)
        {
            targetPlayerForRay = reserveVarTarget;
            instanceIDtarget = reserveVarIDtarget;
        }
        //turnBossToTarget();

        bool enemyIsDefending;

        switch (targetPlayerForRay.tag)
        {
            case "Tank":
                enemyIsDefending = targetPlayerForRay.transform.GetComponent<TankProfile>().shieldActive;

                break;

            case "Mage":
                enemyIsDefending = targetPlayerForRay.transform.GetComponent<MageProfile>().defenseActive;
                break;

            default:
                enemyIsDefending = false;
                break;
        }
        if (!enemyIsDefending)
        {
            Vector3 verticalAdj = new Vector3(transform.position.x, targetPlayerForRay.transform.position.y, transform.position.z);
            Vector3 toBossPos = (verticalAdj - targetPlayerForRay.transform.position);
            targetPlayerForRay.transform.LookAt(verticalAdj);


            isAttracting = true;
            targetPlayerForRay.GetComponent<moreSpecificProfile>().publicSetPreviousStatus(1);
            targetPlayerForRay.GetComponent<moreSpecificProfile>().impulseFromRay((transform.position - targetPlayerForRay.GetComponent<Rigidbody>().transform.position).magnitude, m_PlayerMask);
        }
        else
        {
            targetPlayerForRay = null;
            instanceIDtarget = 0;
            //Debug.Log("FALLITO RAY STA DIFENDENDO");
        }

    }


    public void AoEAttack()
    {

        goAoE = Instantiate(containerAoEAttk, AoEAttackPosition.position, transform.rotation, gameObject.transform);
        isCastingAoE = true;
        StartCoroutine(castingAoEAttack());
    }

    public IEnumerator castingAoEAttack()
    {
        yield return new WaitForSeconds(AoEDuration);
        isCastingAoE = false;
        isAttacking = false;
        isUsingAoE = false;
        Destroy(goAoE);

    }


    public void swingAttack()
    {
        goSwing = Instantiate(swordSwingAttk, swingAttackPosition.position, transform.rotation, gameObject.transform);

        codeAttack = 0;
        StartCoroutine(waitBeforeRemoveSword(1));
    }

    public void aheadAttack()
    {
        if (targetPlayer == null)
        {
            targetPlayer = reserveVarTarget;
            instanceIDtarget = reserveVarIDtarget;
        }
        goAhead = Instantiate(swordAheadAttk, aheadAttackPosition.position, transform.rotation, gameObject.transform);

        Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, goAhead.transform.position.y, targetPlayer.transform.position.z);

        goAhead.transform.LookAt(verticalAdj);
        codeAttack = 1;
        StartCoroutine(waitBeforeRemoveSword(0));
    }


    public IEnumerator waitBeforeRemoveSword(int code)
    {
        yield return new WaitForSeconds(1.0f);

        isAttacking = false;
        targetPlayer = null;
        instanceIDtarget = 0;
        if (code == 1)
        {
            Destroy(goSwing.gameObject);
            isUsingAoE = false;
        }
        else
        {
            if (null != goAhead)
            {
                Destroy(goAhead.gameObject);
            }
            else
            {
                Destroy(goBreak.gameObject);
            }
        }
    }

    public void breakAttack()//wounds that limits healing and reduce armor for tot sec.
    {
        if (targetPlayer == null)
        {
            targetPlayer = reserveVarTarget;
            instanceIDtarget = reserveVarIDtarget;
        }

        goBreak = Instantiate(swordAheadAttk, aheadAttackPosition.position, transform.rotation, gameObject.transform);



        Vector3 verticalAdj = new Vector3(targetPlayer.transform.position.x, goBreak.transform.position.y, targetPlayer.transform.position.z);

        goBreak.transform.LookAt(verticalAdj);
        targetPlayer.GetComponent<moreSpecificProfile>().applyWound();//apply wounds
        targetPlayer.GetComponent<moreSpecificProfile>().reduceArmor();//apply armor reduction
        codeAttack = 2;
        //danno basso
        StartCoroutine(waitBeforeRemoveSword(0));
    }


    public IEnumerator timeBeforeCastSwingAttk()
    {
        yield return new WaitForSeconds(0.4f);
        removePreviousGo();
        swingAttack();

    }
    public IEnumerator timeBeforeCastAheadAttk()
    {
        yield return new WaitForSeconds(0.4f);
        removePreviousGo();
        aheadAttack();

    }
    public IEnumerator timeBeforeCastBreakAttk()
    {
        yield return new WaitForSeconds(0.4f);
        removePreviousGo();
        breakAttack();
    }

    public IEnumerator timeBeforeCastAoEAttk()
    {
        yield return new WaitForSeconds(0.4f);
        removePreviousGo();
        AoEAttack();

    }

    public void turnBossToTargetForRanged()
    {
        Vector3 verticalAdj = new Vector3(originalPositionTarget.x, transform.position.y, originalPositionTarget.z);

        transform.LookAt(verticalAdj);
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
        isCastingAoE = false;
        isAttracting = false;
        isAttacking = false;
        StopAllCoroutines();


    }


    public void removePreviousGo()
    {
        if (null != goSwing)
        {
            Destroy(goSwing.gameObject);
        }
        if (null != goAhead)
        {
            Destroy(goAhead.gameObject);
        }
        if (null != goBreak)
        {
            Destroy(goBreak.gameObject);
        }
        if (null != goAoE)
        {
            Destroy(goAoE.gameObject);
        }
    }














    public bool swingRayCastControll()//quando ha fatto catena fino a ray e tira swing su mage o healer se c'e' alemno un altro champ nello spazio di uso dello swing con raycast
    {
        m_HitDetect_swing_right = Physics.BoxCast(swingAttackPosition.position, transform.localScale, transform.right, out m_Hit_swing_right, transform.rotation, swordSwingAttk.transform.localScale.x/2, m_PlayerMask);
        m_HitDetect_swing_left = Physics.BoxCast(swingAttackPosition.position, transform.localScale, -transform.right, out m_Hit_swing_left, transform.rotation, swordSwingAttk.transform.localScale.x/2, m_PlayerMask);

        if (!m_HitDetect_swing_right && !m_HitDetect_swing_left)
        {
            return false;
        }
        else
        {
            if (m_HitDetect_swing_right)
            {
                if (m_Hit_swing_right.collider.gameObject.GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0)
                {
                    return true;
                }
            }

            if(m_HitDetect_swing_left)
            {
                if (m_Hit_swing_left.collider.gameObject.GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0)//see if the ray hit is an alive champ
                {
                    return true;
                }
            }
            return false;
           
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

    public int targetInAoErange()
    {
        int cont = 0;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 11.0f, m_PlayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            moreSpecificProfile targetProfile = colliders[i].GetComponent<moreSpecificProfile>();

            if (targetProfile.getStatusLifeChamp() == 0)
            {
                cont++;
            }


        }
        return cont;
    }

    public void bossDeath()//BOSS DEAD END EPISODE
    {
        if (GetComponent<moreSpecificProfile>().publicGetCurrentLife()==0 && GetComponent<moreSpecificProfile>().getStatusLifeChamp()==1 & champsKO<4)
        {
            Debug.Log("===== END EPISODE BOSS DEAD =======");
            //StopCoroutine(this.co);
            //StopCoroutine(re);
            //myProfile.endEpStopAll();


            endEpStopAll();

            overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            if (countRewardRun >= 19)
            {
                overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
            }
            else
            {
                overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            }
           

            //moreSpecificProfile[] listOfagents = FindObjectsOfType<moreSpecificProfile>();
            
            foreach (GameObject go in endArray)
            {
                if (!go.transform.tag.Equals("Boss"))
                {
                    //mr.detonation();
                    go.GetComponent<moreSpecificProfile>().detonation();
                }

            }

            GetComponent<moreSpecificProfile>().setFlaResetEpisode(true);
            actionTarget = null;
            //this.AddReward(-1.0f);
            EndEpisode();
        }
        


    }



    public void adjustPlayerArray(GameObject[] newArrayPlay)/// use that when a player is KO to reduce the number of players in the array    
    {
        champsKO++;
        previousTargetID = 0;
        //if (null == newArrayPlay )//se KO ALL PLAYERS
        if(champsKO == 4)
        {
            if (GetComponent<moreSpecificProfile>().publicGetCurrentLife() >= 0 && GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0) { 
                Debug.Log("==== PARTY HA PERSO =====");
                //StopCoroutine(this.co);
                //StopCoroutine(re);
                //myProfile.endEpStopAll();


                endEpStopAll();


                overcomeBattleSignEndRun.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.green);

                if (countRewardRun >= 19)
                {
                    overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
                }
                else
                {
                    overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                }

                //moreSpecificProfile[] listOfagents = FindObjectsOfType<moreSpecificProfile>();

                foreach (GameObject go in this.endArray)
                {
                    if (!go.transform.tag.Equals("Boss"))
                    {
                        //mr.detonation();
                        go.GetComponent<moreSpecificProfile>().detonation();
                    }

                }

                GetComponent<moreSpecificProfile>().setFlaResetEpisode(true);
                actionTarget = null;
                //this.AddReward(1.0f);
                EndEpisode();
            }
        }
        else
        {
            playersParty = newArrayPlay;

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
        Vector3 verticalAdj = new Vector3(playersParty[target].transform.position.x, transform.position.y, playersParty[target].transform.position.z);

        transform.LookAt(verticalAdj);
    }


    public void takeTheAction()
    {
        this.RequestDecision();
        Academy.Instance.EnvironmentStep();
    }

}
