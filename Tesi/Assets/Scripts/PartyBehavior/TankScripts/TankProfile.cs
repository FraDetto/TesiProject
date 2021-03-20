using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProfile : MonoBehaviour
{
    private float hp;
    private float damage;
    private float armor;
    private GameObject sword;
    private GameObject shield;
    public Transform swordTransform;
    private Rigidbody myRB;

    private PartyData partyData;

    // Start is called before the first frame update
    void Start()
    {
        hp = partyData.hpTank;
        damage = partyData.damageTank;
        armor = partyData.armorTank;

    }



    public void attackWithSword()
    {
        Debug.Log("USO LA SPADA");
        swordTransform.transform.localPosition = Vector3.Slerp(swordTransform.localPosition, new Vector3(1, 0, 1), 0.01f);
    }

    public void calculateDamage()
    {

    }

    public void defendWithShield()
    {

    }

    public void activateUlti()
    {

    }

}
