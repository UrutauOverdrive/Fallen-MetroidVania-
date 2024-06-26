using UnityEngine;

public class BossController : MonoBehaviour
{
    private Animator animator;

    public Rigidbody2D rb2D;

    public Transform player;

    private bool lookingRight = true;

    [Header("Life")]

    [SerializeField] private float Life;

    [SerializeField] private BossHPBar lifeBar;

    [Header("Attack")]

    [SerializeField] private Transform attackController;

    [SerializeField] private float attackRadios;

    [SerializeField] private float attackDamage;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        lifeBar.BootLifeBar(Life);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        float playerDistance = Vector2.Distance(transform.position, player.position);
        animator.SetFloat("playerDistance", playerDistance);
    }

    public void TakeDamage(float damage)
    {
        Life -= damage;
        lifeBar.ChangeCurrentLife(Life);

        if (Life <= 0)
        {
            animator.SetTrigger("Death");
        }
    }

    private void Death()
    {
        Destroy(gameObject);
    }

    public void LookAtPlayer()
    {
        if ((player.position.x > transform.position.x && !lookingRight) || (player.position.x < transform.position.x && lookingRight))
        {
            lookingRight = !lookingRight;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }
    }

    public void Attack()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(attackController.position, attackRadios);

        foreach (Collider2D colision in objetos)
        {
            if (colision.CompareTag("Player"))
            {
                colision.GetComponent<PlayerController>().TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackController.position, attackRadios);
    }
}
