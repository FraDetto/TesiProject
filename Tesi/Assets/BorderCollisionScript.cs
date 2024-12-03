using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderCollisionScript : MonoBehaviour
{

    public float m_HealRadius = 6.0f;
    public LayerMask m_PlayerMask;


    public void OnTriggerEnter(Collider other)
    {
        if (null != other.GetComponent<moreSpecificProfile>() && !other.transform.tag.Equals("Boss"))
        {
            Debug.Log("COLLISIONEEE " + other.transform.position);
            other.transform.position = takeRandomPos();
        }

    }

    public Vector3 takeRandomPos()
    {
        bool flag = true;
        float randomX = 0.0f;
        float randomZ = 0.0f;

        while (flag)
        {
            randomX = Random.Range(-35.0f, +35.0f);
            randomZ = Random.Range(-10.0f, -60.0f);

            Collider[] colliders = Physics.OverlapSphere(new Vector3(randomX, 3.5f, randomZ), m_HealRadius, m_PlayerMask);
            if (colliders.Length == 0)
            {
                flag = false;
            }

        }
        return new Vector3(randomX, 3.5f, randomZ);


    }
}
