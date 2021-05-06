using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BossBehavior : Agent
{
    public GameObject[] playersParty;

    [SerializeField] private string partyList;
    private BossProfile myProfile;
    private Vector3 startPosition;

    private GameObject targetForAttack;

    private bool chainRanged = false;
    private bool chainRay = false;

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

        /// Number of targets 
        int target = Mathf.FloorToInt(vectorAction[0]);

        targetForAttack = playersParty[target];

        int actionForBoss = Mathf.FloorToInt(vectorAction[1]);

        myProfile.hubAttacks(actionForBoss, targetForAttack); 


        ////RANGED ATTACK////
        ///Max rew: if used against mage( for bait defense spell ) and After use the ray attack
        ///Good rew: against healer and After use the ray attack
        ///Bad rew: if use against tank and bruiser OR if after bait spelldef to Mage it attacks an other champ

        ////RAY ATTACK////
        ///Max rew: on mage after ranged attack which baits def spell  OR agaisnt healer ONLY if after the ray --> attack that implies the same target
        ///little bad: if do on mage with def up and it defends
        ///Bad rew: against bruiser or tank
        ///

        //// ATTACK AoE ////
        ///Max rew: vs 3 or more target
        ///good rew: vs 2 target and ranged pg are dead
        ///bad rew: vs 1 champ OR there are the 2 melee but there are ranged champ alive 
        ///

        //// Swing Attack ////
        ///Max rew: 2 or more enemies in range of attack && one of them is a rnaged player || 2 enemies and ranged are dead
        ///Ok: if only 1 enemie in range and the previous attack is a ahead 
        ///bad : 1 or 2 enemies none of them ranged && ranged alive
        ///

        //// Ahead Attack ////
        ///Max: target in range with low HP OR on Bruiser if tank alive && ranged dead || on a target after break Attack 
        ///Ok: root champ  --> concluding the chains from rayAttack (so ahead attack on the correct target of ray)
        ///Bad: Attack tank or bruiser if healer alive OR attack tank while bruiser alive
        ///

        //// Break Attack ////
        ///Max: root champ  --> concluding the chains from rayAttack (so break attack on the correct target of ray) OR start a good chain on bruiser if ranged dead OR start a good chain on tank if all dead
        ///OK:
        ///Bad: start a chain : next attack on a different champ or used on tank while bruiser alive and ranged dead OR used on melee while ranged are alive
        ///

    }

    public IEnumerator timeValueReawrd()
    {
        //ricordarsi di gestire i cooldown
        yield return new WaitForSeconds(0.8f);
        valueAndApplyReward();
    }

    public void valueAndApplyReward()
    {

    }

    public void adjustPlayerArray()
    {

    }
}
