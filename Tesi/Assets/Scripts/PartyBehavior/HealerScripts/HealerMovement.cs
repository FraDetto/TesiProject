using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerMovement : MonoBehaviour
{
    private GameObject boss;
    private Rigidbody myRB;

    [SerializeField]private float distanceRangeDown;
    [SerializeField]private float distanceRangeUp;

    public float speed = 15.0f;
    public bool chaseFlag = false;
    public bool distanceFlag = false;
    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");

        myRB = GetComponent<Rigidbody>();

        distanceRangeDown = GetComponent<HealerBehavior>().distanceRangeDown;
        distanceRangeUp = GetComponent<HealerBehavior>().distanceRangeUp;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            if (distanceFlag)
            {
                Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
                Vector3 toBossPos = (verticalAdj - transform.position);

                if ((boss.transform.position - myRB.transform.position).magnitude < distanceRangeDown)
                {
                    transform.LookAt(verticalAdj);
                    myRB.MovePosition(transform.position + (-transform.forward) * speed * Time.deltaTime);
                }
            }
            else
            {
                distanceFlag = false;
            }

            if (chaseFlag)
            {
                Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
                Vector3 toBossPos = (verticalAdj - transform.position);

                if ((boss.transform.position - myRB.transform.position).magnitude > distanceRangeUp)
                {
                    transform.LookAt(verticalAdj);
                    myRB.MovePosition(transform.position + (transform.forward) * speed * Time.deltaTime);
                }
            }
            else
            {
                chaseFlag = false;
            }

        }
    }
}
