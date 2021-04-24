using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class BossBehavior : Agent
{

    [SerializeField] private string partyList;
    private BossProfile myProfile;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        myProfile = GetComponent<BossProfile>();
    }

    public override void OnEpisodeBegin()
    {
      //The episode end when the boss dies or all the party die? I want to do in this way to let the agent understand if an action is ok or not in the long time so ill'give a small rewaerd for an action or a 
      //a sequence of actions that are correct and follow the strategies to defeat the single members of the party and the whole party at the end.
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
    }

    public override void OnActionReceived(float[] vectorAction)
    {
       //action he will apply are discrete (for the moment):the academy generate an int that corresponds to an action of the boss

    }
}
