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

    [SerializeField] private string partyList;
    private BossProfile myProfile;
    private Vector3 startPosition;

    private GameObject targetForAttack;

    private bool chainRanged = false;
    private bool chainRay = false;

    private bool breakBefore = false;

    private int previousRangedTargetID = 0;
    private int previousRayTarget;

    private int[] actionChoose = new int[1];

    private float bonusFutureReward;

    private bool m_HitDetect_swing_right;
    private bool m_HitDetect_swing_left;

    RaycastHit m_Hit_swing_right;
    RaycastHit m_Hit_swing_left;

    // Start is called before the first frame update
    void Start()
    {
        myProfile = GetComponent<BossProfile>();
    }

    public override void OnEpisodeBegin()
    {
        //The episode end when the boss dies or all the party die? I want to do in this way to let the agent understand if an action is ok or not in the long time so ill'give a small rewaerd for an action or a 
        //a sequence of actions that are correct and follow the strategies to defeat the single members of the party and the whole party at the end.
        //
        //At the beginnning of an episode party members are chosen randomly  to enhance the boss's learning
        moreSpecificProfile[] listOfagents = FindObjectsOfType<moreSpecificProfile>();

        foreach(moreSpecificProfile mr in listOfagents)
        {
            if (!mr.transform.tag.Equals("Boss"))
            {
                Destroy(mr);
            }
            else
            {
                myProfile.resetBossStats();
            }
        }

        playersParty = FindObjectOfType<GameManager>().getPartyInGame();
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
        for(int i=0; i< playersParty.Length; i++)
        {
            sensor.AddObservation(playersParty[i].transform.position);
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().publicGetIsDefending());
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().getDefUsed());
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().publicGetCurrentLife());
            sensor.AddObservation(playersParty[i].GetComponent<moreSpecificProfile>().getStatusLifeChamp());
            sensor.AddObservation(playersParty[i]);
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
        Debug.Log(" =====OnActionReceived===== ");
        /// Number of targets 
        int target = Mathf.FloorToInt(vectorAction[0]);

        targetForAttack = playersParty[target];

        int actionForBoss = Mathf.FloorToInt(vectorAction[1]);

        actionChoose[0] = actionForBoss;

        myProfile.hubAttacks(actionForBoss, targetForAttack);


        StartCoroutine(timeValueReward(actionForBoss));
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
        yield return new WaitForSeconds(0.8f);
        valueAndApplyReward(actionForBoss);
    }
    
    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        Debug.Log(" =====SET MASK===== " + actionChoose);
        actionMasker.SetMask(1, actionChoose);
    }

    public void valueAndApplyReward(int actionForBoss)
    {
        Debug.Log(" =====VALUE REWARD===== " + actionChoose);
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
                        if (targetForAttack.tag.Equals("Mage") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                        {
                            if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetIsDefending()) //chain for mage starts only if he used defense spell?
                            {
                                previousRangedTargetID = targetForAttack.GetInstanceID();
                                bonusFutureReward = 0.04f;
                            }//se non difende nujlla neutro
                        }
                        else if (targetForAttack.tag.Equals("Healer") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                        {
                            previousRangedTargetID = targetForAttack.GetInstanceID();
                        }
                        else
                        {
                            this.AddReward(-0.1f);//neg reward: ranged attack on Bruiser or Tank
                        }
                    }
                }
                else if (actionForBoss == 1)//RAY ATTACK
                {
                    if (targetForAttack.tag.Equals("Mage") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                    {
                        if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetIsDefending()) //chain for mage starts only if he used defense spell?
                        {
                            chainRanged = false;
                            previousRangedTargetID = 0;
                            this.AddReward(-0.05f);
                        }
                        else
                        {
                            chainRay = true;
                            
                            //qua mettere il fatto che non puo' usare stessa azione dopo
                        }
                    }
                    else if (targetForAttack.tag.Equals("Healer") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                    {
                        chainRay = true;
                        //qua mettere il fatto che non puo' usare stessa azione dopo
                    }
                    else
                    {
                        chainRanged = false;
                        previousRangedTargetID = 0;
                        this.AddReward(-0.1f);
                    }
                }
                else
                {
                    chainRanged = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.05f);
                }

                
            }
            else
            {
                if (actionForBoss == 0)//RANGED ATTACK
                {
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.1f);
                }else if (actionForBoss == 1)//RAY
                {
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    this.AddReward(-0.1f);
                }
                else if (actionForBoss == 2)//SWING ATTACK
                {
                    if (targetForAttack.tag.Equals("Mage") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                    {
                        if (swingRayCastControll())
                        {
                            this.AddReward(0.08f + bonusFutureReward);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        else
                        {
                            this.AddReward(-0.03f );
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        
                    }
                    else if (targetForAttack.tag.Equals("Healer") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                    {
                        if (swingRayCastControll())
                        {
                            this.AddReward(0.08f + bonusFutureReward);
                            chainRanged = false;
                            chainRay = false;
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        else
                        {
                            this.AddReward(-0.03f);
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
                        this.AddReward(-0.1f);
                    }
                }
                else if (actionForBoss == 3)//AHEAD ATTACK
                {
                    if (targetForAttack.tag.Equals("Mage") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                    {
                        this.AddReward(0.1f + bonusFutureReward);
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else if (targetForAttack.tag.Equals("Healer") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                    {
                        this.AddReward(0.1f + bonusFutureReward);
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else
                    {
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                        this.AddReward(-0.12f);
                    }
                }
                else if (actionForBoss == 4)//BREAK ATTACK
                {
                    if (targetForAttack.tag.Equals("Mage") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                    {
                        this.AddReward(0.06f + bonusFutureReward);
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else if (targetForAttack.tag.Equals("Healer") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                    {
                        this.AddReward(0.06f + bonusFutureReward);
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else
                    {
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                        this.AddReward(-0.12f);
                    }
                }
                else// ACTION 5 AoE
                {
                    if(targetInAoErange() >= 3)
                    {
                        this.AddReward(0.07f + bonusFutureReward);
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
                        this.AddReward(0.01f);
                    }
                    else
                    {
                        chainRanged = false;
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                        this.AddReward(-0.12f);
                    }
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
                this.AddReward(-0.05f);
            }
            else if (actionForBoss == 2)//SWING ATTACK
            {
                if (targetForAttack.tag.Equals("Mage") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                {
                    if (swingRayCastControll())
                    {
                        this.AddReward(0.05f + bonusFutureReward);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else
                    {
                        this.AddReward(-0.03f);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }

                }
                else if (targetForAttack.tag.Equals("Healer") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                {
                    if (swingRayCastControll())
                    {
                        this.AddReward(0.05f + bonusFutureReward);
                        chainRay = false;
                        previousRangedTargetID = 0;
                        bonusFutureReward = 0.0f;
                    }
                    else
                    {
                        this.AddReward(-0.03f);
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
                    this.AddReward(-0.1f);
                }
            }
            else if (actionForBoss == 3)//AHEAD ATTACK
            {
                if (targetForAttack.tag.Equals("Mage") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                {
                    this.AddReward(0.07f + bonusFutureReward);
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
                else if (targetForAttack.tag.Equals("Healer") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                {
                    this.AddReward(0.07f + bonusFutureReward);
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
                else
                {
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                    this.AddReward(-0.1f);
                }
            }
            else if (actionForBoss == 4)//BREAK ATTACK
            {
                if (targetForAttack.tag.Equals("Mage") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                {
                    this.AddReward(0.04f + bonusFutureReward);
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
                else if (targetForAttack.tag.Equals("Healer") && targetForAttack.GetInstanceID() == previousRangedTargetID)
                {
                    this.AddReward(0.04f + bonusFutureReward);
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                }
                else
                {
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                    this.AddReward(-0.1f);
                }
            }
            else /// ATTACK 5 AoE
            {
                if (targetInAoErange() >= 3)
                {
                    this.AddReward(0.03f + bonusFutureReward);
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
                    this.AddReward(0.01f);
                }
                else
                {
                    chainRanged = false;
                    chainRay = false;
                    previousRangedTargetID = 0;
                    bonusFutureReward = 0.0f;
                    this.AddReward(-0.1f);
                }
            }
        }
        else
        {
            if (actionForBoss == 0)// RANGED ATTACK
            {
                if (targetForAttack.tag.Equals("Mage"))
                {
                    if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetIsDefending()) //chain for mage starts only if he used defense spell?
                    {
                        chainRanged = true;
                        previousRangedTargetID = targetForAttack.GetInstanceID();
                        bonusFutureReward = 0.04f;
                    }//se non difende nujlla neutro
                }
                else if (targetForAttack.tag.Equals("Healer"))
                {
                    chainRanged = true;
                    previousRangedTargetID = targetForAttack.GetInstanceID();
                }
                else
                {
                    this.AddReward(-0.05f);//neg reward: ranged attack on Bruiser or Tank
                }
                //qua mettere il fatto che non puo' usare stessa azione dopo
            }
            else if (actionForBoss == 1)//RAY ATTACK
            {
                if (targetForAttack.tag.Equals("Mage"))
                {
                    if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetIsDefending()) //chain for mage starts only if he used defense spell?
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.05f);
                    }
                    else
                    {
                        chainRay = true;
                        bonusFutureReward = 0.02f;
                        previousRangedTargetID = targetForAttack.GetInstanceID();
                        //qua mettere il fatto che non puo' usare stessa azione dopo
                    }
                }
                else if (targetForAttack.tag.Equals("Healer"))
                {
                    chainRay = true;
                    bonusFutureReward = 0.02f;
                    previousRangedTargetID = targetForAttack.GetInstanceID();
                }
                else
                {
                    this.AddReward(-0.08f);//neg reward: ranged attack on Bruiser or Tank
                    previousRangedTargetID = 0;
                }
                //qua mettere il fatto che non puo' usare stessa azione dopo
            }
            else if (actionForBoss == 2)//SWING ATTACK
            {
                if ( targetForAttack.tag.Equals("Bruiser") || targetForAttack.tag.Equals("Tank"))
                {
                    if ( rangedChampAlive() )
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.1f);
                    }
                    else
                    {
                        if(targetForAttack.tag.Equals("Bruiser"))
                        {
                            if (targetForAttack.GetInstanceID() == previousRangedTargetID)
                            {
                                if (breakBefore)// attacked Bruiser with a break Attack before
                                {
                                    if (swingRayCastControll())
                                    {
                                        breakBefore = false;
                                        this.AddReward(0.12f);
                                    }
                                    else
                                    {
                                        breakBefore = false;
                                        this.AddReward(0.04f);
                                    }
                                }
                                else// attacked Bruiser with a NON break Attack before
                                {
                                    if (swingRayCastControll())
                                    {
                                        breakBefore = false;
                                        this.AddReward(0.1f);
                                    }
                                    else
                                    {
                                        breakBefore = false;
                                        this.AddReward(0.03f);
                                    }
                                }
                            }
                            else
                            {
                                if(previousRangedTargetID == 0)
                                {
                                    if (swingRayCastControll())
                                    {
                                        previousRangedTargetID = targetForAttack.GetInstanceID();
                                        this.AddReward(0.08f);
                                    }
                                    else
                                    {
                                        previousRangedTargetID = targetForAttack.GetInstanceID();
                                        this.AddReward(0.02f);
                                    }
                                }
                                else
                                {
                                    breakBefore = false;
                                    previousRangedTargetID = 0;
                                    this.AddReward(-0.1f);
                                }
                            }
                            
                        }
                        else if(targetForAttack.tag.Equals("Tank"))
                        {
                            if (bruiserAlive())
                            {
                                breakBefore = false;
                                previousRangedTargetID = 0;
                                this.AddReward(-0.1f);
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
                                            this.AddReward(0.12f);
                                        }
                                        else
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.04f);
                                        }
                                    }
                                    else
                                    {
                                        if (swingRayCastControll())
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.1f);
                                        }
                                        else
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.03f);
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
                                            this.AddReward(0.08f);
                                        }
                                        else
                                        {
                                            previousRangedTargetID = targetForAttack.GetInstanceID();
                                            this.AddReward(0.02f);
                                        }
                                    }
                                    else
                                    {
                                        breakBefore = false;
                                        previousRangedTargetID = 0;
                                        this.AddReward(-0.1f);
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
                    this.AddReward(-0.1f);
                }
            }
            else if (actionForBoss == 3)//AHEAD ATTACK
            {
                if (targetForAttack.tag.Equals("Bruiser") || targetForAttack.tag.Equals("Tank"))
                {
                    if (rangedChampAlive())
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.1f);
                    }
                    else
                    {
                        if (targetForAttack.tag.Equals("Bruiser"))
                        {
                            if (targetForAttack.GetInstanceID() == previousRangedTargetID)
                            {
                                if (breakBefore)// attacked Bruiser with a break Attack before
                                {
                                    if (!swingRayCastControll())
                                    {
                                        breakBefore = false;
                                        this.AddReward(0.12f);
                                    }
                                    else
                                    {
                                        if(targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife()/100*40))//if low HP
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.12f);
                                        }
                                        else
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.04f);
                                        }
                                        
                                    }
                                }
                                else// attacked Bruiser with a NON break Attack before
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
                                            this.AddReward(0.03f);
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
                                        this.AddReward(0.08f);
                                    }
                                    else
                                    {
                                        if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.08f);
                                        }
                                        else
                                        {
                                            breakBefore = false;
                                            this.AddReward(0.02f);
                                        }

                                    }
                                }
                                else
                                {
                                    breakBefore = false;
                                    previousRangedTargetID = 0;
                                    this.AddReward(-0.1f);
                                }
                            }

                        }
                        else if (targetForAttack.tag.Equals("Tank"))
                        {
                            if (bruiserAlive())
                            {
                                breakBefore = false;
                                previousRangedTargetID = 0;
                                this.AddReward(-0.1f);
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
                                            this.AddReward(0.12f);
                                        }
                                        else
                                        {
                                            if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
                                            {
                                                breakBefore = false;
                                                this.AddReward(0.12f);
                                            }
                                            else
                                            {
                                                breakBefore = false;
                                                this.AddReward(0.04f);
                                            }

                                        }
                                    }
                                    else
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
                                                this.AddReward(0.03f);
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
                                            this.AddReward(0.08f);
                                        }
                                        else
                                        {
                                            if (targetForAttack.GetComponent<moreSpecificProfile>().publicGetCurrentLife() <= (targetForAttack.GetComponent<moreSpecificProfile>().publicGetTotalLife() / 100 * 40))//if low HP
                                            {
                                                breakBefore = false;
                                                this.AddReward(0.08f);
                                            }
                                            else
                                            {
                                                breakBefore = false;
                                                this.AddReward(0.02f);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        breakBefore = false;
                                        previousRangedTargetID = 0;
                                        this.AddReward(-0.1f);
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
                    this.AddReward(-0.1f);
                }
            }
            else if (actionForBoss == 4)//BREAK ATTACK  ricordare di settare correttamente i flag di break a false anche in ranged e ray sopra quando fa casino
            {
                if (targetForAttack.tag.Equals("Bruiser") || targetForAttack.tag.Equals("Tank"))
                {
                    if (rangedChampAlive())
                    {
                        previousRangedTargetID = 0;
                        this.AddReward(-0.1f);
                    }
                    else
                    {
                        if (targetForAttack.tag.Equals("Bruiser"))
                        {
                            if(previousRangedTargetID == 0)
                            {
                                breakBefore = true;
                                previousRangedTargetID = targetForAttack.GetInstanceID();
                            }
                            else if(targetForAttack.GetInstanceID() == previousRangedTargetID)
                            {
                                if (!breakBefore)
                                {
                                    breakBefore = true;
                                    this.AddReward(0.06f);
                                }
                                else
                                {
                                    breakBefore = false;
                                    this.AddReward(-0.05f);
                                }                                
                            }
                            else
                            {
                                previousRangedTargetID = 0;
                                breakBefore = false;
                                this.AddReward(-0.1f);
                            }
                        }
                        else // case of tank
                        {
                            if (targetForAttack.tag.Equals("Tank"))
                            {
                                if (bruiserAlive())
                                {
                                    breakBefore = false;
                                    previousRangedTargetID = 0;
                                    this.AddReward(-0.1f);
                                }
                                else
                                {
                                    if (previousRangedTargetID == 0)
                                    {
                                        breakBefore = true;
                                        previousRangedTargetID = targetForAttack.GetInstanceID();
                                    }
                                    else if (targetForAttack.GetInstanceID() == previousRangedTargetID)
                                    {
                                        if (!breakBefore)
                                        {
                                            breakBefore = true;
                                            this.AddReward(0.06f);
                                        }
                                        else
                                        {
                                            breakBefore = false;
                                            this.AddReward(-0.05f);
                                        }
                                    }
                                    else
                                    {
                                        previousRangedTargetID = 0;
                                        breakBefore = false;
                                        this.AddReward(-0.1f);
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
                    this.AddReward(-0.1f);
                }
            }
            else // ACTION 5 AoE attack
            {
                if (rangedChampAlive())
                {
                    previousRangedTargetID = 0;
                    breakBefore = false;
                    this.AddReward(-0.1f);
                }
                else
                {
                    if (breakBefore)
                    {
                        if (targetInAoErange() >= 3)
                        {
                            this.AddReward(0.1f);
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            breakBefore = false;
                        }
                        else if (targetInAoErange() >= 1 && targetInAoErange() <= 2)
                        {
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            this.AddReward(0.01f);
                            breakBefore = false;
                        }
                        else
                        {
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            this.AddReward(-0.1f);
                            breakBefore = false;
                        }
                        //next action can't be that
                    }
                    else
                    {
                        if (targetInAoErange() >= 3)
                        {
                            this.AddReward(0.05f);
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                        }
                        else if (targetInAoErange() >= 1 && targetInAoErange() <= 2)
                        {
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            this.AddReward(0.01f);
                        }
                        else
                        {
                            previousRangedTargetID = 0;
                            bonusFutureReward = 0.0f;
                            this.AddReward(-0.1f);
                        }
                    }
                }
            }
            
        }
        
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
            return true;
        }       
    }

    public bool rangedChampAlive()
    {
        bool flag = false;

        for(int i=0; i<playersParty.Length; i++)
        {
            if(playersParty[i].tag.Equals("Mage") || playersParty[i].tag.Equals("Healer"))
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
            if (playersParty[i].tag.Equals("Bruiser"))
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, 11.0f, m_PlayerMask);

        return colliders.Length;
    }

    public void adjustPlayerArray()
    {

    }
}
