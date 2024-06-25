using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHPBar : MonoBehaviour
{
    private Slider Slider;

    private void Start()
    {
        Slider = GetComponent<Slider>();
    }

    public void ChangeMaxLife(float maxLife)
    {
        Slider.value = maxLife;
    }

    public void ChangeCurrentLife(float lifeAmount)
    {
        Slider.value = lifeAmount;  
    }

    public void BootLifeBar(float lifeAmount)
    {
        ChangeMaxLife(lifeAmount);
        ChangeCurrentLife(lifeAmount);
    }
}
