using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] prefabsArray;
    public bool makeBetterTeam;

    // Start is called before the first frame update
    void Start()
    {
        if (!makeBetterTeam)
        {
            int[] team = chooseTeam();
            for (int i = 0; i < 4; i++)
            {
                Debug.Log(team[i]);
            }
        }
        else
        {
            int[] team = { 0, 1, 2, 3 };
            for (int i = 0; i < 4; i++)
            {
                Debug.Log(team[i]);
            }
        }
       

    }
    
    public int[] chooseTeam()
    {
        int n;
        int[] team = new int[4];
        for (int i = 0; i < 4; i++){
            n = Random.RandomRange(0, 4);
            team[i] = n;
        }
        
        return team;
    }
}
