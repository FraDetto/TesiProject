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
     
    }

    public override void CollectObservations(VectorSensor sensor)
    {
      
    }

    public override void OnActionReceived(float[] vectorAction)
    {
       
    }
}
