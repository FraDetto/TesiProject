using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] poolOfCLasses;
    public GameObject[] partyOnRun = new GameObject[4];
    public float m_HealRadius = 6.0f;
    public LayerMask m_PlayerMask;

    // Start is called before the first frame update
    void Start()
    {

        //chooseTeam();

    }

    public Vector3 takeRandomPos(int n)
    {
        bool flag = true;
        float randomX = 0.0f;
        float randomZ = 0.0f;

        while (flag)
        {
            randomX = Random.Range(-35.0f, 35.0f);
            randomZ = Random.Range(-10.0f, -60.0f);
            if (n == 0)
            {
                flag = false;
            }
            else
            {
                Collider[] colliders = Physics.OverlapSphere(new Vector3(randomX, 2.8f, randomZ), m_HealRadius, m_PlayerMask);
                if (colliders.Length == 0)
                {
                    flag = false;
                }
            }
        }
        return new Vector3(randomX, 2.8f, randomZ);


    }

    public GameObject[] chooseTeam()
    {
       for(int n=0; n< partyOnRun.Length; n++)
        {
            int rand = Random.Range(0, poolOfCLasses.Length);

            Vector3 spawmPos = takeRandomPos(n);

            partyOnRun[n] = Instantiate(poolOfCLasses[rand], spawmPos, Quaternion.identity);
            Debug.Log("ID GIOCATORI " + partyOnRun[n].GetInstanceID() + " TAG " + partyOnRun[n].tag);
        }
        return partyOnRun;
    }


    public GameObject[] getPartyInGame()
    {
        return chooseTeam();
    }
 
}
