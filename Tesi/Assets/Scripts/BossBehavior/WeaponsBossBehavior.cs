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
            if (!other.GetComponent<moreSpecificProfile>().publicGetIsDefending() && other.GetComponent<moreSpecificProfile>().getStatusLifeChamp()==0)
            {
                int codeAttack = gameObject.GetComponentInParent<BossAttackBehavior>().codeAttack;
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
            }else
            {
                Debug.Log("PLAYER SI STA DIFENDENDO SDOOOOONG");
            }
            
        }
        

    }
}
