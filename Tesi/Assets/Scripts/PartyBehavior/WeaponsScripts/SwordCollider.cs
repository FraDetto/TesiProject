using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    private float damageCharacter = 0.0f;

    public void OnTriggerEnter(Collider other)
    {
        string characterName = transform.parent.tag;

        if (null != gameObject.GetComponentInParent<moreSpecificProfile>())
        {
            switch (characterName)
            {
                case "Tank":
                    damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
                    break;

                default:
                    damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
                    if (gameObject.GetComponentInParent<BruiserProfile>().ultiRunning)
                    {
                        damageCharacter += 35.0f;
                    }
                    break;
            }

            if (other.tag.Equals("Boss"))
            {
                other.transform.gameObject.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(damageCharacter);
            }
        }
        

       
    }
}
