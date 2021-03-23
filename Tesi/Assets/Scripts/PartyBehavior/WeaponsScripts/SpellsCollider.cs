using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellsCollider : MonoBehaviour
{
    private float damageCharacter = 0.0f;

    public void OnTriggerEnter(Collider other)
    {
        string characterName = transform.parent.tag;
      
        switch (characterName)
        {
            case "Mage":
                damageCharacter = gameObject.GetComponentInParent<MageProfile>().getDamage();
                gameObject.GetComponentInParent<MageProfile>().shooting = false;
                break;

            default:
                damageCharacter = gameObject.GetComponentInParent<HealerProfile>().getDamage();
                gameObject.GetComponentInParent<HealerProfile>().shooting = false;
                break;
        }

        if (other.tag.Equals("Boss"))
        {
            other.GetComponent<BossProfile>().takeDamageFromSpell(damageCharacter);
        }

        
        Destroy(this.gameObject);

    }
}
