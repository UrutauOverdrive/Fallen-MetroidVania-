using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class BossController : Enemy
{
    private Animator animator;

    public Rigidbody2D rb2D;

    public Transform player;

    private bool lookingRight = true;

    [Header("Attack")]
    [SerializeField] private Transform attackController;

    [SerializeField] private float attackRadius;

    [SerializeField] private float attackDamage;
    [Space(5)]

    [Header("HP")]
    [SerializeField] private float life;
    //[SerializeField] private HPBar hpBar;//

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        float playerDistance = Vector2.Distance(transform.position, player.position);
        animator.SetFloat("playerDistance", playerDistance);
    }

    /*public void TakeDamage(float _damage)
    {
        life -= _damage;

        if (life <= 0)
        {
            animator.SetTrigger("Death");
        }
    }*/

    public override void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyGetsHit(_damageDone, _hitDirection, _hitForce);
        if (life > 0)
        {
            ChangeState(EnemyStates.Boss_Hurt);
        }
        else
        {
            ChangeState(EnemyStates.Boss_Death);
        }
    }

    /*private void Death()
    {
        Destroy(gameObject);
    }*/

    
    protected override void UpdateEnemyStats()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Boss_Hurt:
               
                break;

            case EnemyStates.Boss_Death:
                anim.SetTrigger("Death");
                Death(Random.Range(5, 10));
                break;
        }
    }
    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }
    public void LookAtPlayer()
    {
        if ((player.position.x > transform.position.x && !lookingRight) || (player.position.x < transform.position.x && lookingRight))
        {
            lookingRight = !lookingRight;
            transform.eulerAngles = new Vector3(0 , transform.eulerAngles.y + 180, 0);
        }
    }

    public void Attack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(attackController.position, attackRadius);      
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackController.position, attackRadius);
    }
}
