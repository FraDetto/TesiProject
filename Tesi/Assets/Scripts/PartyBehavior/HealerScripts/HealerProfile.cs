using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerProfile : MonoBehaviour
{
    private float totalhp;
    [SerializeField] private float currenthp;
    [SerializeField] private float shield;

    [SerializeField] private float damage;
    [SerializeField] private float armor;

    public bool shooting = false;
    private bool cooldownAttack = false;

    public bool cooldownHeal = false;
    public bool cooldownSpecial = true;
    public bool healActive = false;

    private GameObject boss;

    private GameObject tank;
    private GameObject bruiser;
    private GameObject mage;

    private Transform pointSpawnWindBall;
    private Transform pointSpawnHealHealer;
    private Rigidbody myRB;
    private GameObject go;

    public GameObject windBall;


    public float m_HealRadius = 30f;
    public LayerMask m_PlayerMask;

    public PartyData partyData;

    public float speed = 25.0f;

    // Start is called before the first frame update
    void Start()
    {
        totalhp = partyData.hpHealer;

        currenthp = totalhp;
        damage = partyData.damageHealer;
        armor = partyData.armorHealer;

        pointSpawnWindBall = transform.GetChild(1);
        pointSpawnHealHealer = transform.GetChild(2);

        boss = GameObject.FindGameObjectWithTag("Boss");

        tank = GameObject.FindGameObjectWithTag("Tank");
        mage = GameObject.FindGameObjectWithTag("Mage");
        bruiser = GameObject.FindGameObjectWithTag("Bruiser");
    }

    public float getDamage()
    {
        return damage;
    }


    public void addLifeByCure(float cure)
    {
        currenthp += cure;
    }

    private void FixedUpdate()
    {
        if (shooting)
        {
            go.transform.LookAt(boss.transform.position);
            go.transform.position += go.transform.forward * speed * Time.deltaTime;
        }
    }

    public void attackWithMagic()
    {
        //Debug.Log("attackWithMagic");
        if (!cooldownAttack)
        {
            go = Instantiate(windBall, pointSpawnWindBall.position, transform.rotation, gameObject.transform);
            shooting = true;
            cooldownAttack = true;
            StartCoroutine(waitAfterAttack());
        }

    }

    public IEnumerator waitAfterAttack()
    {

        yield return new WaitForSeconds(2.0f);
        cooldownAttack = false;
    }

    public void calculateDamage()
    {

    }

    public void healAlly()
    {
        findAllyToHeal();
        StartCoroutine(waitBeforeRemoveShield());
        StartCoroutine(cooldownDefense());
    }

    public void findAllyToHeal()
    {
       
    }

    public void calculateHeal()
    {

    }

    public IEnumerator waitBeforeRemoveShield()
    {
        yield return new WaitForSeconds(2.0f);
        healActive = false;
        Destroy(go);
    }

    public IEnumerator cooldownDefense()
    {
        yield return new WaitForSeconds(10.0f);
        cooldownHeal = false;
    }


    public void activateUlti()
    {

    }
}
