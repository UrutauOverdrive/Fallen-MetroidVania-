using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMagic : MonoBehaviour
{
    [SerializeField] private float damage;

    [SerializeField] private Vector2 magicDimension;

    [SerializeField] private Transform magicPosition;

    [SerializeField] private float lifetime;

    private void Start()
    {
        Destroy(gameObject, lifetime);        
    }

    public void Strike()
    {
        Collider2D[] objects = Physics2D.OverlapBoxAll(magicPosition.position, magicDimension, 0f);
        /*
        foreach (Collider2D collision in objects)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<PlayerController>().TakeDamage(attackDamage);
            }
        }
        */
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(magicPosition.position, magicDimension);
    }
}
