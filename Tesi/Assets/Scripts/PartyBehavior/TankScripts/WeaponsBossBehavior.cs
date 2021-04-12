using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsBossBehavior : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        //sword
        if (!other.tag.Equals("Boss"))
        {
            float damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();

            other.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(damageCharacter);
        }
        

    }
}
