using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHPBar : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void ChangeMaxLife(float maxLife)
    {
        slider.maxValue = maxLife;
    }

    public void ChangeCurrentLife(float lifeAmount)
    {
        slider.value = lifeAmount;  
    }

    public void BootLifeBar(float lifeAmount)
    {
        ChangeMaxLife(lifeAmount);
        ChangeCurrentLife(lifeAmount);
    }
}
