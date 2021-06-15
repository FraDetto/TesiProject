using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    
    private GameObject boss;
    private Rigidbody rb;

    public float distanceRange = 7.0f;
    public float speed = 15.0f;
    public bool chaseFlag = false;

    private float maxAngle = 45f;
    private float currentAngleR = 0f;
    private float currentAngleL = 0f;


    private bool m_HitDetect_mov_front;

    RaycastHit m_Hit_mov_front;
    public LayerMask m_PlayerMask;
    // Start is called before the first frame update
    void Start()
    {
        //boss = GameObject.FindGameObjectWithTag("Boss");

        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GetComponent<moreSpecificProfile>().flagResetepisode)
        {
            if (chaseFlag)
            {
                Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
                Vector3 toBossPos = (verticalAdj - transform.position);

                //if (toBossPos.magnitude > distanceRange)
                if ((boss.transform.position - rb.transform.position).magnitude > distanceRange)
                {
                    /*m_HitDetect_mov_front = Physics.BoxCast(transform.position, transform.localScale, (boss.transform.position - rb.transform.position), out m_Hit_mov_front, transform.rotation, (boss.transform.position - rb.transform.position).magnitude, m_PlayerMask);
                    if (m_HitDetect_mov_front)
                    {
                        if (currentAngleR + 30.0f <= maxAngle)
                        {
                            Vector3 m_EulerAngleVelocity = new Vector3(0f, 30.0f, 0f);
                            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
                            rb.MoveRotation(rb.rotation * deltaRotation);
                            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
                            currentAngleR += 30f;
                            //good smooth rotation in the other branch
                        }
                        else
                        {
                            if (boss.transform.rotation.y - transform.rotation.y <= 90f)
                            {
                                if (currentAngleL + 15.0f <= maxAngle)
                                {
                                    Vector3 m_EulerAngleVelocity = new Vector3(0f, -15.0f, 0f);
                                    Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
                                    rb.MoveRotation(rb.rotation * deltaRotation);
                                    rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
                                    currentAngleL += 15f;
                                    //good smooth rotation in the other branch
                                }
                                else
                                {
                                    rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
                                }
                            }
                            else
                            {
                                rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
                            }
                            
                        }

                    }
                    else
                    {
                        //transform.LookAt(verticalAdj);
                        var targetRotation = Quaternion.LookRotation(boss.transform.position - rb.transform.position);

                        // Smoothly rotate towards the target point.
                        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, 5.0f * Time.deltaTime));
                        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
                    }*/
                    var targetRotation = Quaternion.LookRotation(boss.transform.position - rb.transform.position);

                    // Smoothly rotate towards the target point.
                    rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, 10.0f * Time.deltaTime));
                    rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
                }
                else
                {
                    chaseFlag = false;
                    currentAngleR = 0f;
                    currentAngleL = 0f;
                }
            }
        }   
       
    }
    /*
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (m_HitDetect_mov_front)
            {
                Gizmos.DrawRay(transform.position, transform.forward * m_Hit_mov_front.distance);
                Gizmos.DrawWireCube(transform.position + transform.forward * m_Hit_mov_front.distance, transform.localScale);
            }
            else
            {
                //Draw a Ray forward from GameObject toward the maximum distance
                Gizmos.DrawRay(transform.position, transform.forward * 50.0f);
                //Draw a cube at the maximum distance
                Gizmos.DrawWireCube(transform.position + transform.forward * 50.0f, transform.localScale);
            }
        }
    */

    public void setBoss(GameObject bo)
    {
        boss = bo;
    }

}
