using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerMovement : MonoBehaviour
{
    private GameObject boss;
    private Rigidbody myRB;

    public float distanceRange = 15.0f;
    public float speed = 15.0f;
    public bool chaseFlag = false;
    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");

        myRB = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (chaseFlag)
        {
            Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
            Vector3 toBossPos = (verticalAdj - transform.position);

            if ((boss.transform.position - myRB.transform.position).magnitude < distanceRange)
            {
                transform.LookAt(verticalAdj);
                myRB.MovePosition(transform.position + (-transform.forward) * speed * Time.deltaTime);
            }
        }

    }
}
