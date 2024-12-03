using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shieldBarScript : MonoBehaviour
{

    public Slider slider;

    public void setMaxShield(float shieldValue)
    {
        slider.maxValue = shieldValue;
        slider.value = shieldValue;
    }

    public void setShield(float shieldValue)
    {
        slider.value = shieldValue;
    }
}
