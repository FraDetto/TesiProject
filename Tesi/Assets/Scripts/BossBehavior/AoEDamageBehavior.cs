using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEDamageBehavior : MonoBehaviour
{
    public GameObject[] playersHit = new GameObject[4];
    public int cont = 0;

    public void OnTriggerEnter(Collider other)
    {
        //bool flag = false;
        float damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
        if (null!=other.GetComponent<moreSpecificProfile>())
        {
            if (other.GetComponent<moreSpecificProfile>().getStatusLifeChamp() == 0 && !other.GetComponent<moreSpecificProfile>().publicGetIsDefending() )
            {
                other.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(((damageCharacter / 100) * 55));
                /*
                for (int i = 0; i < playersHit.Length; i++)
                {
                    if (null != playersHit && other.gameObject.GetInstanceID() == playersHit[i].GetInstanceID())
                    {
                        flag = true;

                    }
                }

                if (!flag)
                {
                    playersHit[cont] = other.gameObject;

                    //other.transform.gameObject.GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage( ((damageCharacter/100)*50) );
                    cont++;
                }*/
            }
            
        }

       
    }

    /*
    void OnDestroy()
    {
        float damageCharacter = gameObject.GetComponentInParent<moreSpecificProfile>().publicGetDamageValue();
        ///danno
        for (int i = 0; i < cont; i++)
        {
            if (!playersHit[i].GetComponent<moreSpecificProfile>().publicGetIsDefending())
            {
                playersHit[i].GetComponent<moreSpecificProfile>().publicSetLifeAfterDamage(((damageCharacter / 100) * 50));
            }
             
       
        }
    }*/
}
