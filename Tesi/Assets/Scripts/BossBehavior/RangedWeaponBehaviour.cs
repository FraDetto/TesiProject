using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponBehaviour : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (null != other.GetComponent<moreSpecificProfile>() && !other.transform.tag.Equals("Boss"))
        {
            if (!other.GetComponent<moreSpecificProfile>().publicGetIsDefending() && other.GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0)
            {
                float damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();

                this.transform.GetComponentInParent<BossProfile>().isShooting = false;
                other.transform.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(damageCharacter);
                this.transform.GetComponentInParent<BossProfile>().isAttacking = false;

                this.transform.GetComponentInParent<BossProfile>().targetPlayer = null;
                this.transform.GetComponentInParent<BossProfile>().instanceIDtarget = 0;
                //this.transform.GetComponentInParent<BossProfile>().target = "";
            }
            else
            {
                if (other.GetComponent<moreSpecificProfile>().publicGetIsDefending())
                {
                    Debug.Log("PLAYER SI STA DIFENDENDO SDDDDDDDDDDOOOOOONG SPLAASH");
                    this.transform.GetComponentInParent<BossProfile>().isShooting = false;
                    this.transform.GetComponentInParent<BossProfile>().isAttacking = false;
                    this.transform.GetComponentInParent<BossProfile>().targetPlayer = null;
                    this.transform.GetComponentInParent<BossProfile>().instanceIDtarget = 0;
                    //this.transform.GetComponentInParent<BossProfile>().target = "";
                }


            }
           
            Destroy(this.gameObject);
        }
        else
        {
            if (other.tag.Equals("Ground"))
            {
                this.transform.GetComponentInParent<BossProfile>().isShooting = false;
                this.transform.GetComponentInParent<BossProfile>().isAttacking = false;
                this.transform.GetComponentInParent<BossProfile>().targetPlayer = null;
                this.transform.GetComponentInParent<BossProfile>().instanceIDtarget = 0;
                //this.transform.GetComponentInParent<BossProfile>().target = "";
                Destroy(this.gameObject);
            }
        }
       
        
    }
}
