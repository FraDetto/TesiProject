using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class aProfile : MonoBehaviour
{
    //

    protected abstract float getCurrentLife();

    protected abstract float getTotalLife();

    protected abstract float getDamageValue();

    protected abstract int getStatus();

    protected abstract void setLifeAfterDamage(float damage);

    protected abstract void addLifeByCure(float cure);

    protected abstract void addShield(float shieldValue);

    protected abstract void resetShield();

    protected abstract void addRootStatus(float rootDuration);

    protected abstract void addStunStatus(float stunDuration);


}
