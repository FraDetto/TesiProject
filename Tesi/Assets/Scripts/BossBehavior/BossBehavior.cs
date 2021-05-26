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

    public GameManager gameManager;

    public bool firstRun = true;

    public int champsKO;

    [SerializeField] private string partyList;
    private BossProfile myProfile;
    private Vector3 startPosition;

    private GameObject targetForAttack;

    private bool chainRanged = false;
    private bool chainRay = false;

    private bool breakBefore = false;

    private int previousRangedTargetID = 0;

    private int[] actionChoose = new int[1];
    private int[] actionTarget;

    private float bonusFutureReward;

    private bool m_HitDetect_swing_right;
    private bool m_HitDetect_swing_left;

    RaycastHit m_Hit_swing_right;
    RaycastHit m_Hit_swing_left;

    private GameObject[] arrayForEnd;

    //TYPECODE PLAYERS 0 TANK, 1 BRUISER, 2 MAGE, 3 HEALER


    Coroutine co;
    Coroutine re;

    // Start is called before the first frame update
    void Start()
    {
        myProfile = GetComponent<BossProfile>();
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
        Debug.Log(" =====OnEPISODE BEGIN===== ");
        

        if (!firstRun)
        {
            playersParty = gameManager.generatePartyInGame();
            myProfile.assignPartyForProfile();
            GetComponent<moreSpecificProfile>().resetBossStats();
            takeTheAction();
        }
        else
        {
            playersParty = gameManager.generatePartyInGame();
            myProfile.assignPartyForProfile();
        }

        champsKO = 0;
        arrayForEnd = playersParty;

        

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
        sensor.AddObservation(transform.position);
        sensor.AddObservation(GetComponent<moreSpecificProfile>().publicGetCurrentLife());
        sensor.AddObservation(GetComponent<moreSpecificProfile>().getStatusLifeChamp());
        sensor.AddObservation(champsKO);
        sensor.AddObservation(chainRanged);
        sensor.AddObservation(chainRay);
        sensor.AddObservation(previousRangedTargetID);
        sensor.AddObservation(targetInAoErange());
        sensor.AddObservation(swingRayCastControll());
        sensor.AddObservation(rangedChampAlive());

        for (int i=0; i< playersParty.Length; i++)
        {
            sensor.AddObservation(playersParty[i].transform.position);
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().publicGetIsDefending());
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().getDefUsed());
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().publicGetCurrentLife());
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().getStatusLifeChamp());
            sensor.AddObservation(playersParty[i].GetInstanceID());
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().getTypeCode());
        }
        
        /*
        sensor.AddObservation(playersParty[1].transform.position);
        sensor.AddObservation(playersParty[1].GetComponent<moreSpecificProfile>().publicGetIsDefending());
        sensor.AddObservation(playersParty[1].GetComponent<moreSpecificProfile>().publicGetCurrentLife());
        sensor.AddObservation(playersParty[1].GetComponent<moreSpecificProfile>().getStatusLifeChamp());
        sensor.AddObservation(playersParty[1]);

        sensor.AddObservation(playersParty[2].transform.position);
        sensor.AddObservation(playersParty[2].GetComponent<moreSpecificProfile>().publicGetIsDefending());
        sensor.AddObservation(playersParty[2].GetComponent<moreSpecificProfile>().publicGetCurrentLife());
        sensor.AddObservation(playersParty[2].GetComponent<moreSpecificProfile>().getStatusLifeChamp());
        sensor.AddObservation(playersParty[2]);

        sensor.AddObservation(playersParty[3].transform.position);
        sensor.AddObservation(playersParty[3].GetComponent<moreSpecificProfile>().publicGetIsDefending());
        sensor.AddObservation(playersParty[3].GetComponent<moreSpecificProfile>().publicGetCurrentLife());
        sensor.AddObservation(playersParty[3].GetComponent<moreSpecificProfile>().getStatusLifeChamp());
        sensor.AddObservation(playersParty[3]);*/

        
        
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

        Debug.Log(" =====OnActionReceived===== " + " TARGET " + vectorAction[0] + " AZIONE " + vectorAction[1]);
        /// Number of targets 
        int target = Mathf.FloorToInt(vectorAction[0]);

        targetForAttack = playersParty[target];

        int actionForBoss = Mathf.FloorToInt(vectorAction[1]);

        actionChoose[0] = actionForBoss;

        Debug.Log("PLAYER PER PROSSIMO ATTACCO " + targetForAttack.tag + " CON ID "+ targetForAttack.GetInstanceID());

        turnBossToTarget();

        myProfile.hubAttacks(actionForBoss, targetForAttack);


        re = StartCoroutine(timeValueReward(actionForBoss));


        

        //StartCoroutine(timeBeforeAnOtherAction());


        //// 0 RANGED ATTACK////  
        ///Max rew: if used against mage( for bait defense spell ) and After use the ray attack
        ///Good rew: against healer and After use the ray attack
        ///Bad rew: if use against tank and bruiser OR if after bait spelldef to Mage it attacks an other champ OR on Healer and after not use ray on Healer

        //// 1 RAY ATTACK////
        ///Max rew: on mage after ranged attack which baits def spell  OR agaisnt healer ONLY if after the ray --> attack that implies the same target
        ///little bad: if do on mage with def up and it defends
        ///Bad rew: against bruiser or tank
        ///

        //// 2 ATTACK AoE ////
        ///Max rew: vs 3 or more target
        ///good rew: vs 2 target and ranged pg are dead
        ///bad rew: vs 1 champ OR there are the 2 melee but there are ranged champ alive 
        ///

        //// 3 Swing Attack ////
        ///Max rew: 2 or more enemies in range of attack && one of them is a rnaged player || 2 enemies and ranged are dead
        ///Ok: if only 1 enemie in range and the previous attack is a ahead 
        ///bad : 1 or 2 enemies none of them ranged && ranged alive
        ///

        //// 4 Ahead Attack ////
        ///Max: target in range with low HP OR on Bruiser if tank alive && ranged dead || on a target after break Attack 
        ///Ok: root champ  --> concluding the chains from rayAttack (so ahead attack on the correct target of ray)
        ///Bad: Attack tank or bruiser if healer alive OR attack tank while bruiser alive
        ///

        //// 5 Break Attack ////
        ///Max: root champ  --> concluding the chains from rayAttack (so break attack on the correct target of ray) OR start a good chain on bruiser if ranged dead OR start a good chain on tank if all dead
        ///OK:
        ///Bad: start a chain : next attack on a different champ or used on tank while bruiser alive and ranged dead OR used on melee while ranged are alive
        ///

    }

    public IEnumerator timeValueReward(int actionForBoss)
    {
        //ricordarsi di gestire i cooldown
        yield return new WaitForSeconds(0.5f);
        valueAndApplyReward(actionForBoss);
    }

    public IEnumerator timeBeforeAnOtherAction()
    {
        Debug.Log(" =====DOVREBBE CHIAMARE ALTRA AZIONE===== ");
        yield return new WaitForSeconds(2.4f);

        this.RequestDecision();
        Academy.Instance.EnvironmentStep();

        
    }
    
    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        if (!firstRun)
        {
            Debug.Log(" =====SET MASK ACTION ===== " + actionChoose[0]);
            if (null != actionTarget)
            {
                Debug.Log(" =====SET MASK TARGET===== " + actionTarget[0]);
                actionMasker.SetMask(0, actionTarget);
            }         
            actionMasker.SetMask(1, actionChoose);
        }
        else
        {
            Debug.Log(" =====SET MASK FIRST RUN===== " );
            firstRun = false;
        }
       
    }

    public void valueAndApplyReward(int actionForBoss)
    {
        Debug.Log(" =====VALUE REWARD===== " + actionChoose[0]);
        if (chainRanged)
        {
            if (!chainRay)
            {
                if (actionForBoss == 0)//RANGED
                {
                    if (targetForAttack.GetComponent<moreSpecificProfile>().getDefUsed())
                    {
                        Debug.Log(" RI-USATO RANGED----------- " + actionChoose);
                        chainRanged = false;
                        previousRangedTargetID = 0;
                        this.AddReward(-0.1f);
                    }
                    else
                    {
                        //if (targetForAttack.tag.Equals("Mage") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                        if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==2 && targetForAttack.GetInstanceID() == previousRangedTargetID)//MAGE
                        {
                            if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetIsDefending()) //chain for mage starts only if he used defense spell?
                            {
                                previousRangedTargetID = targetForAttack.GetInstanceID();
                                bonusFutureReward = 0.04f;
                            }//se non difende nujlla neutro
                        }
                        else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==3 && targetForAttack.GetInstanceID() == previousRangedTargetID)//HEALER
                        {
                            previousRangedTargetID = targetForAttack.GetInstanceID();
                        }
                        else
                        {
                            this.AddReward(-0.2f);//neg reward: ranged attack on Bruiser or Tank
                        }
                    }
                }
                else if (actionForBoss == 1)//RAY ATTACK
                {
                    if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==2 && targetForAttack.GetInstanceID() == previousRangedTargetID)//MAGE
                    {
                        if (targetForAttack.GetComponent<moreSpecificProfile>().getDefUsed())
                        {
                            chainRay = true;
                            this.AddReward(0.1f);
                        }
                        else
                        {
                            if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetIsDefending()) //chain for mage starts only if he used defense spell?
                            {
                                chainRanged = false;
                                previousRangedTargetID = 0;
                                this.AddReward(-0.1f);
                            }
                            else
                            {
                                chainRay = true;

                                //qua mettere il fatto che non puo' usare stessa azione dopo
                            }
                        }
                       
                    }
                    else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==3 && targetForAttack.GetInstanceID() == previousRangedTargetID)//HEALER
                    {
                        chainRay = true;
                        //qua mettere il fatto che non puo' usare stessa azione dopo
                        this.AddReward(0.1f);
                    }
                    else
                    {
                        chainRanged = false;
                        previousRangedTargetID = 0;
                        this.AddReward(-0.25f);
                    }
                }
                else
                {
                    chainRanged = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.2f);
                }

                
            }
            else
            {
                if (actionForBoss == 0)//RANGED ATTACK
                {
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.2f);
                }else if (actionForBoss == 1)//RAY
                {
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.2f);
                }
                else if (actionForBoss == 2)//SWING ATTACK
                {
                    if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==2 && targetForAttack.GetInstanceID() == previousRangedTargetID)//MAGE
                    {
                        if (swingRayCastControll())
                        {
                            this.AddReward(0.1f + bonusFutureReward);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        else
                        {
                            this.AddReward(-0.1f );
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        
                    }
                    else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==3 && targetForAttack.GetInstanceID() == previousRangedTargetID)//HEALER
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
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                        this.AddReward(-0.2f);
                    }
                }
                else if (actionForBoss == 3)//AHEAD ATTACK
                {
                    if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==2 && targetForAttack.GetInstanceID() == previousRangedTargetID)//MAGE
                    {
                        if (!swingRayCastControll())
                        {
                            this.AddReward(0.12f + bonusFutureReward);
                        }
                        else
                        {
                            this.AddReward(-0.12f);
                        }
                    }
                    else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==3 && targetForAttack.GetInstanceID() == previousRangedTargetID)//HEALER
                    {
                        if (!swingRayCastControll())
                        {
                            this.AddReward(0.12f + bonusFutureReward);
                        }
                        else
                        {
                            this.AddReward(-0.12f);

                        }
                    }
                    else
                    {
                        this.AddReward(-0.2f);
                    }

                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
                else if (actionForBoss == 4)//BREAK ATTACK
                {
                    if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==2 && targetForAttack.GetInstanceID() == previousRangedTargetID)//MAGE
                    {
                        this.AddReward(0.08f + bonusFutureReward);
                    }
                    else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==3 && targetForAttack.GetInstanceID() == previousRangedTargetID)//HEALER
                    {
                        this.AddReward(0.08f + bonusFutureReward);
                    }
                    else
                    {
                        this.AddReward(-0.2f);
                    }
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
                else// ACTION 5 AoE
                {
                    /*   if (targetInAoErange() >= 3)
                       {
                           this.AddReward(0.08f + bonusFutureReward);
                           chainRanged = false;
                           chainRay = false;
                           previousRangedTargetID = 0;
                           bonusFutureReward = 0.0f;
                       }else if (targetInAoErange() >= 1 && targetInAoErange()<=2)
                       {
                           chainRanged = false;
                           chainRay = false;
                           previousRangedTargetID = 0;
                           bonusFutureReward = 0.0f;
                           this.AddReward(-0.04f);
                       }
                       else
                       {
                           chainRanged = false;
                           chainRay = false;
                           previousRangedTargetID = 0;
                           bonusFutureReward = 0.0f;
                           this.AddReward(-0.2f);
                       }*/
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                    this.AddReward(-0.15f);
                }
            }
            
            
        }
        else if (chainRay)
        {
            if (actionForBoss == 0)//RANGED ATTACK
            {
                chainRay = false;
                bonusFutureReward = 0.0f;
                previousRangedTargetID = 0;
                this.AddReward(-0.15f);
            }
            else if (actionForBoss == 2)//SWING ATTACK
            {
                if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==2 && targetForAttack.GetInstanceID() == previousRangedTargetID)//MAGE
                {
                    if (swingRayCastControll())
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
                else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==3 && targetForAttack.GetInstanceID() == previousRangedTargetID)//HEALER
                {
                    if (swingRayCastControll())
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
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                    this.AddReward(-0.25f);
                }
            }
            else if (actionForBoss == 3)//AHEAD ATTACK
            {
                if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==2 && targetForAttack.GetInstanceID() == previousRangedTargetID)//MAGE
                {
                    if (!swingRayCastControll())
                    {
                        this.AddReward(0.1f + bonusFutureReward);                       
                    }
                    else
                    {
                        this.AddReward(-0.08f);
                    }
                }
                else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==3 && targetForAttack.GetInstanceID() == previousRangedTargetID)//HEALER
                {
                    if (!swingRayCastControll())
                    {
                        this.AddReward(0.1f + bonusFutureReward);
                    }
                    else
                    {
                        this.AddReward(-0.08f);
                    }
                }
                else
                {
                    this.AddReward(-0.25f);
                }
                chainRay = false;
                previousRangedTargetID = 0;
                bonusFutureReward = 0.0f;
            }
            else if (actionForBoss == 4)//BREAK ATTACK
            {
                if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==2 && targetForAttack.GetInstanceID() == previousRangedTargetID)//MAGE
                {
                    this.AddReward(0.06f + bonusFutureReward);
                    chainRay = false;
                    breakBefore = true;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
                else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode()==3 && targetForAttack.GetInstanceID() == previousRangedTargetID)//HEALER
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
                    this.AddReward(-0.25f);
                }
            }
            else /// ATTACK 5 AoE
            {
                /*   if (targetInAoErange() >= 3)
                   {
                       this.AddReward(0.07f + bonusFutureReward);
                       chainRanged = false;
                       chainRay = false;
                       previousRangedTargetID = 0;
                       bonusFutureReward = 0.0f;
                   }
                   else if (targetInAoErange() >= 1 && targetInAoErange() <= 2)
                   {
                       chainRanged = false;
                       chainRay = false;
                       previousRangedTargetID = 0;
                       bonusFutureReward = 0.0f;
                       this.AddReward(-0.04f);
                   }
                   else
                   {
                       chainRanged = false;
                       chainRay = false;
                       previousRangedTargetID = 0;
                       bonusFutureReward = 0.0f;
                       this.AddReward(-0.25f);
                   }*/
                chainRay = false;
                previousRangedTargetID = 0;
                bonusFutureReward = 0.0f;
                this.AddReward(-0.15f);
            }
        }
        else
        {
            if (actionForBoss == 0)// RANGED ATTACK
            {
                if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 2)//MAGE
                {
                    if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetIsDefending()) //chain for mage starts only if he used defense spell?
                    {
                        chainRanged = true;
                        previousRangedTargetID = targetForAttack.GetInstanceID();
                        bonusFutureReward = 0.04f;
                    }//se non difende nujlla neutro
                }
                else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 3)//HEALER
                {
                    chainRanged = true;
                    previousRangedTargetID = targetForAttack.GetInstanceID();
                }
                else
                {
                    this.AddReward(-0.12f);//neg reward: ranged attack on Bruiser or Tank
                }
                //qua mettere il fatto che non puo' usare stessa azione dopo
            }
            else if (actionForBoss == 1)//RAY ATTACK
            {
                if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 2)//MAGE
                {
                    if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetIsDefending()) //chain for mage starts only if he used defense spell?
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.05f);
                    }
                    else
                    {
                        chainRay = true;
                        bonusFutureReward = 0.06f;
                        this.AddReward(0.05f);
                        previousRangedTargetID = targetForAttack.GetInstanceID();
                        //qua mettere il fatto che non puo' usare stessa azione dopo
                    }
                }
                else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 3)//HEALER
                {
                    chainRay = true;
                    bonusFutureReward = 0.06f;
                    this.AddReward(0.05f);
                    previousRangedTargetID = targetForAttack.GetInstanceID();
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
                if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 1 || targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 0)//BRUISER OR TANK
                {
                    if ( rangedChampAlive() )
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.3f);
                    }
                    else
                    {
                        if(targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 1)//BRUISER
                        {
                            if (targetForAttack.GetInstanceID() == previousRangedTargetID)
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
                                if(previousRangedTargetID == 0)
                                {
                                    if (swingRayCastControll())
                                    {
                                        previousRangedTargetID = targetForAttack.GetInstanceID();
                                        this.AddReward(0.1f);
                                    }
                                    else
                                    {
                                        previousRangedTargetID = targetForAttack.GetInstanceID();
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
                        else if(targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 0)//TANK
                        {
                            if (bruiserAlive())
                            {
                                breakBefore = false;
                                previousRangedTargetID = 0;
                                this.AddReward(-0.2f);
                            }
                            else
                            {
                                if (targetForAttack.GetInstanceID() == previousRangedTargetID)
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
                                            previousRangedTargetID = targetForAttack.GetInstanceID();
                                            this.AddReward(0.1f);
                                        }
                                        else
                                        {
                                            previousRangedTargetID = targetForAttack.GetInstanceID();
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
                    this.AddReward(-0.3f);
                }
            }
            else if (actionForBoss == 3)//AHEAD ATTACK
            {
                if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 1 || targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 0)//BRUISER OR TANK
                {
                    if (rangedChampAlive())
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.3f);
                    }
                    else
                    {
                        if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 1)//BRUISER
                        {
                            if (targetForAttack.GetInstanceID() == previousRangedTargetID)
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
                                        if(targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife()/100*40))//if low HP
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
                                        if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
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
                                        if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
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
                        else if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 0)//TANK
                        {
                            if (bruiserAlive())
                            {
                                breakBefore = false;
                                previousRangedTargetID = 0;
                                this.AddReward(-0.2f);
                            }
                            else
                            {
                                if (targetForAttack.GetInstanceID() == previousRangedTargetID)
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
                                            if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
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
                                            if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
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
                                            if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
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
                    this.AddReward(-0.3f);
                }
            }
            else if (actionForBoss == 4)//BREAK ATTACK  ricordare di settare correttamente i flag di break a false anche in ranged e ray sopra quando fa casino
            {
                if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 1|| targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 0)//BRUISER OR TANK
                {
                    if (rangedChampAlive())
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.3f);
                    }
                    else
                    {
                        if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 1)//BRUISER
                        {
                            if(previousRangedTargetID == 0)
                            {
                                breakBefore = true;
                                previousRangedTargetID = targetForAttack.GetInstanceID();
                                this.AddReward(0.07f);
                            }
                            else if(targetForAttack.GetInstanceID() == previousRangedTargetID)
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
                            if (targetForAttack.GetComponent<moreSpecificProfile>().getTypeCode() == 0)//TANK
                            {
                                if (bruiserAlive())
                                {
                                    breakBefore = false;
                                    previousRangedTargetID = 0;
                                    this.AddReward(-0.2f);
                                }
                                else
                                {
                                    if (previousRangedTargetID == 0)
                                    {
                                        breakBefore = true;
                                        previousRangedTargetID = targetForAttack.GetInstanceID();
                                        this.AddReward(0.07f);
                                    }
                                    else if (targetForAttack.GetInstanceID() == previousRangedTargetID)
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
                    this.AddReward(-0.3f);
                }
            }
            else // ACTION 5 AoE attack
            {
                if (rangedChampAlive())
                {
                    previousRangedTargetID = 0;
                    breakBefore = false;
                    this.AddReward(-0.3f);
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
                            this.AddReward(-0.2f);
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
                            this.AddReward(-0.2f);
                        }
                    }
                }
            }
            
        }

        co = StartCoroutine(timeBeforeAnOtherAction());

    }




    public bool swingRayCastControll()//quando ha fatto catena fino a ray e tira swing su mage o healer se c'e' alemno un altro champ nello spazio di uso dello swing con raycast
    {
        m_HitDetect_swing_right = Physics.BoxCast(myProfile.getSwingAttackPos().position, transform.localScale, transform.right, out m_Hit_swing_right, transform.rotation, swordSwingAttk.transform.localScale.x/2, m_PlayerMask);
        m_HitDetect_swing_left = Physics.BoxCast(myProfile.getSwingAttackPos().position, transform.localScale, -transform.right, out m_Hit_swing_left, transform.rotation, swordSwingAttk.transform.localScale.x/2, m_PlayerMask);

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
        if ((targetForAttack.transform.position - transform.position).magnitude < 10.0f)
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
            StopCoroutine(co);
            StopCoroutine(re);
            myProfile.endEpStopAll();
            overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

            //moreSpecificProfile[] listOfagents = FindObjectsOfType<moreSpecificProfile>();

            foreach (GameObject go in arrayForEnd)
            {
                if (!go.transform.tag.Equals("Boss"))
                {
                    //mr.detonation();
                    go.GetComponent<moreSpecificProfile>().detonation();
                }

            }

            GetComponent<moreSpecificProfile>().setFlaResetEpisode(true);
            actionTarget = null;
            this.AddReward(-5.0f);
            EndEpisode();
        }
        


    }



    public void adjustPlayerArray(GameObject[] newArrayPlay)/// use that when a player is KO to reduce the number of players in the array
    {
        champsKO++;
        if (null == newArrayPlay )//se KO ALL PLAYERS
        {
            if (GetComponent<moreSpecificProfile>().publicGetCurrentLife() >= 0 && GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0 && champsKO==4) { 
                Debug.Log("==== PARTY HA PERSO =====");
                StopCoroutine(co);
                StopCoroutine(re);
                myProfile.endEpStopAll();

                overcomeBattleSign.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.green);

                //moreSpecificProfile[] listOfagents = FindObjectsOfType<moreSpecificProfile>();

                foreach (GameObject go in arrayForEnd)
                {
                    if (!go.transform.tag.Equals("Boss"))
                    {
                        //mr.detonation();
                        go.GetComponent<moreSpecificProfile>().detonation();
                    }

                }

                GetComponent<moreSpecificProfile>().setFlaResetEpisode(true);
                actionTarget = null;
                this.AddReward(5.0f);
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
        Vector3 verticalAdj = new Vector3(targetForAttack.transform.position.x, transform.position.y, targetForAttack.transform.position.z);

        transform.LookAt(verticalAdj);
    }


    public void takeTheAction()
    {
        this.RequestDecision();
        Academy.Instance.EnvironmentStep();
    }

}
