using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProfile : moreSpecificProfile
{

    private bool cooldownSword = false;
    public bool cooldownShield = false;
    public bool cooldownSpecial = true;
    public bool shieldActive = false;
    public bool swordActive = false;

    public bool cooldownDash = false;
    public bool isDashing = false;

    public GameObject sword;
    public GameObject shieldTank;


    private Rigidbody rb;
    private GameObject go;
    private Transform pointSpawnSword;
    private Transform pointSpawnShield;
    private Transform pointSpawnHealHealer;

    
    private float swordDuration = 1.0f;
    private float timeCoolDownSwordAttack = 2.0f;
    private float shieldDuration = 2.0f;
    private float timeCoolDownShield = 10.0f;
    private float specialDuration = 6.0f;
    private float timeCoolDownSpecial = 16.0f;

    private float timeRollCooldown = 2.0f;

    private float dashFroce = 17.0f;


    private bool m_HitDetect_left;
    private bool m_HitDetect_right;
    private bool m_HitDetect_back;

    RaycastHit m_Hit_left;
    RaycastHit m_Hit_right;
    RaycastHit m_Hit_back;

    float m_MaxDistance = 10.0f;
    public LayerMask m_PlayerMask;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        pointSpawnSword = transform.GetChild(1);
        pointSpawnShield = transform.GetChild(2);
        pointSpawnHealHealer = transform.GetChild(3);

        StartCoroutine(waitAfterUlti());
    }




    public void attackWithSword()
    {
        //Debug.Log("attackWithSword");
        if (!cooldownSword)
        {
            go = Instantiate(sword, pointSpawnSword.position, transform.rotation, gameObject.transform);
            cooldownSword = true;
            swordActive = true;
            StartCoroutine(waitBeforeRemoveSword());
            StartCoroutine(cooldownAttack());
        }
    }

    public IEnumerator waitBeforeRemoveSword()
    {
        yield return new WaitForSeconds(swordDuration);
        swordActive = false;
        Destroy(go);
    }

    public IEnumerator cooldownAttack()
    {
        yield return new WaitForSeconds(timeCoolDownSwordAttack);
        cooldownSword = false;
    }



    public void defendWithShield()
    {
        go = Instantiate(shieldTank, pointSpawnShield.position, transform.rotation, gameObject.transform);
        cooldownShield = true;
        shieldActive = true;
        StartCoroutine(waitBeforeRemoveShield());
        StartCoroutine(cooldownDefense());
    }

    public IEnumerator waitBeforeRemoveShield()
    {
        yield return new WaitForSeconds(shieldDuration);
        shieldActive = false;
        Destroy(go);
    }

    public IEnumerator cooldownDefense()
    {
        yield return new WaitForSeconds(timeCoolDownShield);
        cooldownShield = false;
    }

    public void specialInAction()
    {
        transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        cooldownSpecial = true;

        StartCoroutine(waitBeforeStopSpecial());
        
        StartCoroutine(waitAfterUlti());
        
    }

    public IEnumerator waitBeforeStopSpecial()
    {
        //Debug.Log("ULTI STA PERDURANDO");
        yield return new WaitForSeconds(specialDuration);
        transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        //Debug.Log("ULTI FINITA");
    }

    public IEnumerator waitAfterUlti()
    {
        //Debug.Log("ULTI IN COOLDOWN");
        yield return new WaitForSeconds(timeCoolDownSpecial);
        cooldownSpecial = false;
        //Debug.Log("ULTI UP");
    }


    public void rollAway()
    {
        cooldownDash = true;
        isDashing = true; ;

        correctRoll();

        StartCoroutine(endDash());
        StartCoroutine(waitRollCollDown());
    }

    public IEnumerator endDash()
    {
        yield return new WaitForSeconds(1.0f);
        isDashing = false;
    }

    public IEnumerator waitRollCollDown()
    {
        yield return new WaitForSeconds(timeRollCooldown);
        cooldownDash = false;
        rb.velocity = Vector3.zero;
    }


    public void correctRoll()
    {
        Debug.Log("SONO QUA");
        m_HitDetect_left = Physics.BoxCast(transform.position, transform.localScale, -transform.right, out m_Hit_left, transform.rotation, m_MaxDistance, m_PlayerMask);
        m_HitDetect_right = Physics.BoxCast(transform.position, transform.localScale, transform.right, out m_Hit_right, transform.rotation, m_MaxDistance, m_PlayerMask);
        m_HitDetect_back = Physics.BoxCast(transform.position, transform.localScale, -transform.forward, out m_Hit_back, transform.rotation, m_MaxDistance, m_PlayerMask);

        if (!m_HitDetect_left && !m_HitDetect_right && !m_HitDetect_back)
        {
            Debug.Log("LIBERE TUTTE E TRE");
            int way = Random.Range(1, 4);// 1 left, 2 back, 3 right

            switch (way)
            {
                case 1:
                    rb.velocity = -transform.right * dashFroce;
                    break;
                case 2:
                    rb.velocity = -transform.forward * dashFroce;
                    break;
                default:
                    rb.velocity = transform.right * dashFroce;
                    break;
            }
        }
        else if (m_HitDetect_left && !m_HitDetect_right && !m_HitDetect_back)
        {
            Debug.Log("CONTATTO L");
            int way = Random.Range(1, 3);// 1 back, 2 right

            switch (way)
            {
                case 1:
                    rb.velocity = -transform.forward * dashFroce;
                    break;
                default:
                    rb.velocity = transform.right * dashFroce;
                    break;
            }
        } else if (m_HitDetect_left && m_HitDetect_right && !m_HitDetect_back)
        {
            rb.velocity = -transform.forward * dashFroce; //BACK
        } else if (!m_HitDetect_left && m_HitDetect_right && !m_HitDetect_back)
        {
            Debug.Log("CONTATTO R");
            int way = Random.Range(1, 3);// 1 left, 2 back, 

            switch (way)
            {
                case 1:
                    rb.velocity = -transform.right * dashFroce;
                    break;
                default:
                    rb.velocity = -transform.forward * dashFroce;
                    break;

            }
        } else if (!m_HitDetect_left && !m_HitDetect_right && m_HitDetect_back)
        {
            Debug.Log("CONTATTO B");
            int way = Random.Range(1, 3);// 1 left, 2 right, 

            switch (way)
            {
                case 1:
                    rb.velocity = -transform.right * dashFroce;
                    break;
                default:
                    rb.velocity = transform.right * dashFroce;
                    break;

            }

        }
        else if (m_HitDetect_left && !m_HitDetect_right && m_HitDetect_back)
        {
            rb.velocity = transform.right * dashFroce; //RIGHT
        }
        else if (!m_HitDetect_left && m_HitDetect_right && m_HitDetect_back)
        {
            rb.velocity = -transform.right * dashFroce; //LEFT
        }
        else
        {
            //CASE HIT ALL DIRECTION IN THEORY NOT POSSIBLE
            Debug.Log("CASO CHE NON DOVREBBE VERIFICARSI DI COLPITE IN TUTTE LE DIREZIONI PER ROLL ");
        }

        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (m_HitDetect_left)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * m_Hit_left.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * m_Hit_left.distance, transform.localScale);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, -transform.right * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + -transform.right * m_MaxDistance, transform.localScale);
        }

        if (m_HitDetect_right)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.right * m_Hit_right.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.right * m_Hit_right.distance, transform.localScale);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.right * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.right * m_MaxDistance, transform.localScale);
        }

        if (m_HitDetect_back)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, -transform.forward * m_Hit_back.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + -transform.forward * m_Hit_back.distance, transform.localScale);
            
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, -transform.forward * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + -transform.forward * m_MaxDistance, transform.localScale);
        }
    }

}
