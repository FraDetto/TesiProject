using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponBehaviour : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (null != other.GetComponent<moreSpecificProfile>() && !other.transform.tag.Equals("Boss"))
        {
            float damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
            this.transform.GetComponentInParent<BossProfile>().isShooting = false;
            other.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(damageCharacter);

            Destroy(this.gameObject);
        }
        
    }
}
