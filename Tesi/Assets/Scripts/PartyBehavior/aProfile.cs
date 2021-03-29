using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class aProfile : MonoBehaviour
{
    //protected PartyData partyData;
    // Start is called before the first frame update
    /*
    void Start()
    {
        partyData = Resources.Load<PartyData>("../DataManagment/PartyData");
    }
    */
    protected abstract float getCurrentLife();
}
