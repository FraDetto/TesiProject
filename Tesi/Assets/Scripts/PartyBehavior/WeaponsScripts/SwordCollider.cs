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
                damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
                break;          

            default:
                damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();              
                break;
        }

        if (other.tag.Equals("Boss"))
        {
            other.transform.gameObject.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(damageCharacter);
        }

       
    }
}
