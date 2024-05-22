using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator _animator;

    public Rigidbody2D rb2D;

    public Transform player;

    private bool lookingRight = true;

    [Header("HP")]
    [SerializeField] private float life;
    [SerializeField] private HPBar hpBar;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        //hpBar
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
