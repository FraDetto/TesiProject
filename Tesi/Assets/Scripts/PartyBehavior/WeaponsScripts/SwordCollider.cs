using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    private float damageCharacter = 0.0f;

    public void OnTriggerEnter(Collider other)
    {
        string characterName = transform.parent.tag;
       
        switch (characterName)
        {
            case "Tank":
                damageCharacter = gameObject.GetComponentInParent<TankProfile>().getDamage();
                break;          

            default:
                damageCharacter = gameObject.GetComponentInParent<BruiserProfile>().getDamage();
                break;
        }

        if (other.tag.Equals("Boss"))
        {
            other.GetComponent<BossProfile>().takeDamageFromSword(damageCharacter);
        }
        
    }
}
