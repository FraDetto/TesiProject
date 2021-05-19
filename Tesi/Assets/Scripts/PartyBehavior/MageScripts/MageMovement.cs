using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageMovement : MonoBehaviour
{
    private GameObject boss;
    private Rigidbody rb;

    private float distanceRangeDown;
    private float distanceRangeUp;
    public float speed = 15.0f;
    public bool chaseFlag = false;
    public bool distanceFlag = false;

    
    // Start is called before the first frame update
    void Start()
    {
        //boss = GameObject.FindGameObjectWithTag("Boss");

        rb = GetComponent<Rigidbody>();

        distanceRangeDown = GetComponent<MageBehavior>().distanceRangeDown;
        distanceRangeUp = GetComponent<MageBehavior>().distanceRangeUp;
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

                if ((boss.transform.position - rb.transform.position).magnitude < distanceRangeDown)
                {
                    transform.LookAt(verticalAdj);
                    rb.MovePosition(transform.position + (-transform.forward) * speed * Time.deltaTime);
                }
                else
                {
                    distanceFlag = false;
                }
            }

            if (chaseFlag)
            {
                Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
                Vector3 toBossPos = (verticalAdj - transform.position);

                if ((boss.transform.position - rb.transform.position).magnitude > distanceRangeUp)
                {
                    transform.LookAt(verticalAdj);
                    rb.MovePosition(transform.position + (transform.forward) * speed * Time.deltaTime);
                }
                else
                {
                    chaseFlag = false;
                }
            }

            if (transform.GetComponent<MageProfile>().defenseActive)
            {
                if (transform.GetComponent<MageProfile>().rightDefSpellDirection)
                {
                    if (transform.rotation.y <= 0.70f)
                    {

                        Vector3 m_EulerAngleVelocity = new Vector3(0f, 60.0f, 0f);
                        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
                        rb.MoveRotation(rb.rotation * deltaRotation);
                    }

                    rb.MovePosition(transform.position + transform.forward * 12.0f * Time.deltaTime);
                }
                else
                {
                    if (transform.rotation.y >= -0.70f)
                    {
                        Vector3 m_EulerAngleVelocity = new Vector3(0f, -60.0f, 0f);
                        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
                        rb.MoveRotation(rb.rotation * deltaRotation);
                    }

                    rb.MovePosition(transform.position + transform.forward * 12.0f * Time.deltaTime);
                }
            }


        }

    }


    public void setBoss(GameObject bo)
    {
        boss = bo;
    }
}
