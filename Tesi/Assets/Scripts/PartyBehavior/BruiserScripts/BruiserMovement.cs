using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruiserMovement : MonoBehaviour
{
    private GameObject boss;
    private Rigidbody rb;
    private float initialPositionZ;
    private float dashForce = 14.0f;
    private bool flagDash;

    public float valueOfZAfterMovement = 0.0f;

    public float distanceRange = 7.0f;
    public float speed = 15.0f;
    public bool chaseFlag = false;

    public bool directionRight = false;


    private bool m_HitDetect_mov_front;

    RaycastHit m_Hit_mov_front;
    public LayerMask m_PlayerMask;

    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");

        rb = GetComponent<Rigidbody>();

        initialPositionZ = transform.position.z;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (chaseFlag)
        {
            Vector3 verticalAdj = new Vector3(boss.transform.position.x, transform.position.y, boss.transform.position.z);
            Vector3 toBossPos = (verticalAdj - transform.position);


            //if (toBossPos.magnitude > distanceRange)
            if ((boss.transform.position - rb.transform.position).magnitude > distanceRange)
            {
                m_HitDetect_mov_front = Physics.BoxCast(transform.position, transform.localScale, (boss.transform.position - rb.transform.position), out m_Hit_mov_front, transform.rotation, (boss.transform.position - rb.transform.position).magnitude, m_PlayerMask);
                if (m_HitDetect_mov_front)
                {
                    Debug.Log("STO HITTANDO");
                    Vector3 m_EulerAngleVelocity = new Vector3(0f, 30.0f, 0f);
                    Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
                    rb.MoveRotation(rb.rotation * deltaRotation);
                    rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
                    //good smooth rotation in the other branch
                }
                else
                {
                    transform.LookAt(verticalAdj);

                    rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
                }
                //far scegliere un altro punto in cui andare perche alleato davanti

                


               


                valueOfZAfterMovement = initialPositionZ - transform.position.z;
            }
            else
            {
                chaseFlag = false;
            }
        }

    }

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
}
