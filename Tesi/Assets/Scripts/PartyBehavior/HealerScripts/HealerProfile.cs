using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;

    public bool shooting = false;
    private bool cooldown = false;

    private GameObject boss;
    private Transform pointSpawnWindBall;
    private Rigidbody myRB;
    private GameObject go;

    public GameObject windBall;


    public PartyData partyData;

    public float speed = 25.0f;

    // Start is called before the first frame update
    void Start()
    {
        hp = partyData.hpHealer;
        damage = partyData.damageHealer;
        armor = partyData.armorHealer;

        pointSpawnWindBall = transform.GetChild(1);

        boss = GameObject.FindGameObjectWithTag("Boss");
    }

    public float getDamage()
    {
        return damage;
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
        if (!cooldown)
        {
            go = Instantiate(windBall, pointSpawnWindBall.position, transform.rotation, gameObject.transform);
            shooting = true;
            cooldown = true;
            StartCoroutine(waitAfterAttack());
        }

    }

    public IEnumerator waitAfterAttack()
    {

        yield return new WaitForSeconds(2.0f);
        cooldown = false;
    }

    public void calculateDamage()
    {

    }

    public void healAlly()
    {

    }

    public void activateUlti()
    {

    }
}
