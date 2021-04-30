using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsCollider : MonoBehaviour
{
    private float damageCharacter = 0.0f;


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Boss"))
        {
            string characterName = transform.parent.tag;
      
            switch (characterName)
            {
                case "Mage":
                    damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
                    gameObject.GetComponentInParent<MageProfile>().shooting = false;
                    break;

                default:
                    damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
                    gameObject.GetComponentInParent<HealerProfile>().shooting = false;
                    break;
            }

            other.transform.gameObject.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(damageCharacter);
            Destroy(this.gameObject);
        }

        
      

    }
}
