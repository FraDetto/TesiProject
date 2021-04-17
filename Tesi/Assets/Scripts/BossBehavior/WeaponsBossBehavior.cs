using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsBossBehavior : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        //sword
        if (null != other.GetComponent<moreSpecificProfile>() && !other.tag.Equals("Boss"))
        {
            int codeAttack = gameObject.GetComponentInParent<BossProfile>().codeAttack;
            float damageCharacter = 0.0f;

            switch (codeAttack)
            {
                case 0: //swing attack
                    damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
                    break;
                case 1: //ahead attack
                    damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
                    damageCharacter += (damageCharacter / 100 * 40);
                    break;
                default: //break attack
                    damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
                    damageCharacter -= (damageCharacter / 100 * 40);
                    break;
            }

            other.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(damageCharacter);
        }
        

    }
}
