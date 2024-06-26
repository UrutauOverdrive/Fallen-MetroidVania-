using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float Life;

    public void TakeBossDamage(float damage)
    {
        Life -= damage;

        if (Life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
