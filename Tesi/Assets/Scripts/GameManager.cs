using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject boss;
    public GameObject[] poolOfCLasses;
    public GameObject[] partyOnRun = new GameObject[4];
    public GameObject obstaclesGO;
    public GameObject refObstacles;
    public GameObject shieldOb;
    public GameObject refShieldObj;
    public BossMovingBehavior movingBrain;
    public float m_HealRadius = 6.0f;
    public LayerMask m_PlayerMask;
    private int numb;

    private float lastXobj = 0f;

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
            randomZ = Random.Range(boss.transform.position.z-15.0f, boss.transform.position.z-60.0f);

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

    public Vector3 takePosForObstacles()
    {

        float randomX = 0.0f;
        float randomZ = 0.0f;

        
        //randomX = Random.Range(-35.0f, 35.0f);
        //randomZ = Random.Range(-10.0f, -60.0f);
        if(lastXobj > 10f)
        {
            randomX = Random.Range(-60.0f, 0f);
        }
        else if (lastXobj < -10f)
        {
            randomX = Random.Range(0f, 60.0f);
        }
        else
        {
            randomX = Random.Range(-60.0f, 60.0f);
        }

        randomZ = Random.Range(-10.0f, +40.0f);


        lastXobj = randomX;

        return new Vector3(randomX, 4.8f, randomZ);
    }

    
    public void ableRoutineForObstacles()
    {
        Destroy(refObstacles);
        StartCoroutine(respawnObstacles());
    }

    public IEnumerator respawnObstacles()
    {
        yield return new WaitForSeconds(3.0f);
        Vector3 postObs = takePosForObstacles();

        refObstacles = Instantiate(obstaclesGO, postObs, Quaternion.identity, transform.parent);
        refShieldObj = Instantiate(shieldOb, new Vector3(postObs.x, postObs.y - 2, postObs.z + 6), Quaternion.identity, transform.parent);

        movingBrain.setShieldObj(refShieldObj);
    }

    public IEnumerator generateFirstObstacle()
    {
        yield return new WaitForSeconds(8.0f);

        Vector3 postObs = takePosForObstacles();

        refObstacles = Instantiate(obstaclesGO, postObs, Quaternion.identity, transform.parent);
        refShieldObj = Instantiate(shieldOb, new Vector3(postObs.x, postObs.y - 2, postObs.z + 6), Quaternion.identity, transform.parent);
        //sfera messa circa +6 della Z dell'ostacolo
        movingBrain.setShieldObj(refShieldObj);

    }

    public void stopRoutManager()
    {
        StopAllCoroutines();

        if (null != refShieldObj)
        {
            Destroy(refShieldObj);
        }

        if (null != refObstacles)
        {
            Destroy(refObstacles);
        }
    }

    public GameObject[] chooseTeam()
    {
        StartCoroutine(generateFirstObstacle());

        for (int n=0; n< partyOnRun.Length; n++)
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
        int[] a = { 0, 1, 2, 3 };

        for (int n = 0; n < partyOnRun.Length; n++)
        {
            if (a.Length == 1)
            {
                numb = a[0];
            }
            else
            {
                a = removePos(a);
            }
            

            Vector3 spawmPos = takeRandomPos(n);

            partyOnRun[numb] = Instantiate(poolOfCLasses[n], spawmPos, Quaternion.identity, transform.parent);
            // Debug.Log("ID GIOCATORI " + partyOnRun[n].GetInstanceID() + " TAG " + partyOnRun[n].tag);



            switch (partyOnRun[numb].tag)
            {
                case "Tank":
                    partyOnRun[numb].GetComponent<TankBehavior>().setBoss(boss);
                    partyOnRun[numb].GetComponent<TankMovement>().setBoss(boss);
                    partyOnRun[numb].GetComponent<TankProfile>().setBoss(boss);
                    break;

                case "Bruiser":
                    partyOnRun[numb].GetComponent<BruiserBehavior>().setBoss(boss);
                    partyOnRun[numb].GetComponent<BruiserMovement>().setBoss(boss);
                    partyOnRun[numb].GetComponent<BruiserProfile>().setBoss(boss);
                    break;

                case "Mage":
                    partyOnRun[numb].GetComponent<MageBehavior>().setBoss(boss);
                    partyOnRun[numb].GetComponent<MageMovement>().setBoss(boss);
                    partyOnRun[numb].GetComponent<MageProfile>().setBoss(boss);
                    break;

                case "Healer":
                    partyOnRun[numb].GetComponent<HealerBehavior>().setBoss(boss);
                    partyOnRun[numb].GetComponent<HealerMovement>().setBoss(boss);
                    partyOnRun[numb].GetComponent<HealerProfile>().setBoss(boss);
                    break;

                default:
                    Debug.Log("CHARACTER UNKNOWN");
                    break;
            }
            partyOnRun[numb].GetComponent<moreSpecificProfile>().setBossRef(boss);
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

    public int[] removePos(int[] a)// to rondomize the positions in the array
    {
        int rand = Random.Range(0, a.Length);
        int[] newA = new int[a.Length - 1];

        int cont = 0;

        numb = a[rand];

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != numb)
            {
                newA[cont] = a[i];
                cont++;
            }
        }

        return newA;
    }

}
