using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject boss;
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
            //randomX = Random.Range(-35.0f, 35.0f);
            //randomZ = Random.Range(-10.0f, -60.0f);
            randomX = Random.Range(boss.transform.position.x-35.0f, boss.transform.position.x+35.0f);
            randomZ = Random.Range(boss.transform.position.z-10.0f, boss.transform.position.z-60.0f);

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

            partyOnRun[n] = Instantiate(poolOfCLasses[rand], spawmPos, Quaternion.identity, transform.parent);
            //Debug.Log("ID GIOCATORI " + partyOnRun[n].GetInstanceID() + " TAG " + partyOnRun[n].tag);


            switch (partyOnRun[n].tag)
            {
                case "Tank":
                    partyOnRun[n].GetComponent<TankBehavior>().setBoss(boss);
                    partyOnRun[n].GetComponent<TankMovement>().setBoss(boss);
                    partyOnRun[n].GetComponent<TankProfile>().setBoss(boss);
                    break;

                case "Bruiser":
                    partyOnRun[n].GetComponent<BruiserBehavior>().setBoss(boss);
                    partyOnRun[n].GetComponent<BruiserMovement>().setBoss(boss);
                    partyOnRun[n].GetComponent<BruiserProfile>().setBoss(boss);
                    break;

                case "Mage":
                    partyOnRun[n].GetComponent<MageBehavior>().setBoss(boss);
                    partyOnRun[n].GetComponent<MageMovement>().setBoss(boss);
                    partyOnRun[n].GetComponent<MageProfile>().setBoss(boss);
                    break;

                case "Healer":
                    partyOnRun[n].GetComponent<HealerBehavior>().setBoss(boss);
                    partyOnRun[n].GetComponent<HealerMovement>().setBoss(boss);
                    partyOnRun[n].GetComponent<HealerProfile>().setBoss(boss);
                    break;

                default:
                    Debug.Log("CHARACTER UNKNOWN");
                    break;
            }
            partyOnRun[n].GetComponent<moreSpecificProfile>().setBossRef(boss);
        }
        return partyOnRun;
    }

    public GameObject[] takeStandardTeam()
    {
        for (int n = 0; n < partyOnRun.Length; n++)
        {

            Vector3 spawmPos = takeRandomPos(n);

            partyOnRun[n] = Instantiate(poolOfCLasses[n], spawmPos, Quaternion.identity, transform.parent);
           // Debug.Log("ID GIOCATORI " + partyOnRun[n].GetInstanceID() + " TAG " + partyOnRun[n].tag);


            switch (partyOnRun[n].tag)
            {
                case "Tank":
                    partyOnRun[n].GetComponent<TankBehavior>().setBoss(boss);
                    partyOnRun[n].GetComponent<TankMovement>().setBoss(boss);
                    partyOnRun[n].GetComponent<TankProfile>().setBoss(boss);
                    break;

                case "Bruiser":
                    partyOnRun[n].GetComponent<BruiserBehavior>().setBoss(boss);
                    partyOnRun[n].GetComponent<BruiserMovement>().setBoss(boss);
                    partyOnRun[n].GetComponent<BruiserProfile>().setBoss(boss);
                    break;

                case "Mage":
                    partyOnRun[n].GetComponent<MageBehavior>().setBoss(boss);
                    partyOnRun[n].GetComponent<MageMovement>().setBoss(boss);
                    partyOnRun[n].GetComponent<MageProfile>().setBoss(boss);
                    break;

                case "Healer":
                    partyOnRun[n].GetComponent<HealerBehavior>().setBoss(boss);
                    partyOnRun[n].GetComponent<HealerMovement>().setBoss(boss);
                    partyOnRun[n].GetComponent<HealerProfile>().setBoss(boss);
                    break;

                default:
                    Debug.Log("CHARACTER UNKNOWN");
                    break;
            }
            partyOnRun[n].GetComponent<moreSpecificProfile>().setBossRef(boss);
        }
        return partyOnRun;
    }

    public GameObject[] generateStandardPartyInGame()
    {
        return takeStandardTeam();
    }

    public GameObject[] generatePartyInGame()
    {
        return chooseTeam();
    }
 

    public GameObject[] getParty()
    {
        return partyOnRun;
    }


}
