using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;


public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1; //sets the players movement speed on the ground
    [Space(5)]

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45f; //sets how hight the player can jump

    private int jumpBufferCounter = 0; //stores the jump button input
    [SerializeField] private int jumpBufferFrames; //sets the max amount of frames the jump buffer input is stored

    private float coyoteTimeCounter = 0; //stores the Grounded() bool
    [SerializeField] private float coyoteTime; //sets the max amount of frames the Grounded() bool is stored

    private int airJumpCounter = 0; //keeps track of how many times the player has jumped in the air
    [SerializeField] private int maxAirJumps; //the max no. of air jumps

    private float gravity; //stores the gravity scale at start
    [Space(5)]



    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint; //point at which ground check happens
    [SerializeField] private float groundCheckY = 0.2f; //how far down from ground chekc point is Grounded() checked
    [SerializeField] private float groundCheckX = 0.5f; //how far horizontally from ground chekc point to the edge of the player is
    [SerializeField] private LayerMask whatIsGround; //sets the ground layer
    [Space(5)]



    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed; //speed of the dash
    [SerializeField] private float dashTime; //amount of time spent dashing
    [SerializeField] private float dashCooldown; //amount of time between dashes
    
    private bool canDash = true, dashed;
    [Space(5)]



    [Header("Attack Settings:")]
    [SerializeField] private Transform SideAttackTransform; //the middle of the side attack area
    [SerializeField] private Vector2 SideAttackArea; //how large the area of side attack is

    [SerializeField] private Transform UpAttackTransform; //the middle of the up attack area
    [SerializeField] private Vector2 UpAttackArea; //how large the area of side attack is

    [SerializeField] private Transform DownAttackTransform; //the middle of the down attack area
    [SerializeField] private Vector2 DownAttackArea; //how large the area of down attack is

    [SerializeField] private LayerMask attackableLayer; //the layer the player can attack and recoil off of

    [SerializeField] private float timeBetweenAttack;
    private float timeSinceAttack;

    [SerializeField] private float damage; //the damage the player does to an enemy

    

    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]



    [Header("Recoil Settings:")]
    [SerializeField] private int recoilXSteps = 5; //how many FixedUpdates() the player recoils horizontally for
    [SerializeField] private int recoilYSteps = 5; //how many FixedUpdates() the player recoils vertically for

    [SerializeField] private float recoilXSpeed = 100; //the speed of horizontal recoil
    [SerializeField] private float recoilYSpeed = 100; //the speed of vertical recoil

    private int stepsXRecoiled, stepsYRecoiled; //the no. of steps recoiled horizontally and verticall
    [Space(5)]

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;

    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]

    [Header("Stamina Settings")]
    [SerializeField] public Image StaminaBar;
    [SerializeField] public float Stamina;
    [SerializeField] public float MaxStamina;
    [SerializeField] public float attackCost;
    [SerializeField] public float dashCost;
    [SerializeField] public float chargeRate;

    private float rechargeTime;
    private Coroutine recharge;
    [Space(5)]

    [Header("Spell Settings")]
    //spell stats
    [SerializeField] float spellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    float timeSinceCast;
    [SerializeField] float spellDamage; //upspellexplosion and downspellfireball
    [SerializeField] float downSpellForce; // desolate dive only
    //spell cast objects
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;
    float castTimer;
    [Space(5)]

    [Header("Parry Settings")]
    [SerializeField] private float parryWindow = 0.2f; // Janela de tempo para o parry
    private float parryTimer; // Temporizador para o parry
    [SerializeField] private LayerMask enemyLayer; // Camada do inimigo para detecção de colisão
    [SerializeField] private LayerMask obstacleLayer; // Camada do obstáculo para detecção de colisão
    [Space(5)]


    [HideInInspector] public PlayerStateList pState;
    private Animator anim;
    public Rigidbody2D rb;
    private SpriteRenderer sr;

    //Input Variables
    private float xAxis, yAxis;
    private bool attack = false;
    bool openMap;

    private bool canFlash = true;


    //creates a singleton of the PlayerController
    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();


      
        gravity = rb.gravityScale;

        Health = maxHealth;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        ParryInput(); // Verifica o input para parry
        if (pState.cutscene) return;

        if (pState.alive)
        {
            GetInputs();
            ToggleMap();
        }

        UpdateJumpVariables();
        RestoreTimeScale(); 

        if (pState.dashing || pState.healing) return;
        if (pState.alive)
        {
            Move();
            Heal();
            CastSpell();
            Flip();
            Jump();
            StartDash();
            Attack();
        }
       
        FlashWhileInvincible();

        if (Stamina < MaxStamina)
        {
            StartCoroutine(RechargeStamina());

            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
        

    }

    void ParryInput()
    {
        // Se o jogador pressionar o botão de parry
        if (Input.GetKeyDown(KeyCode.X))
        {
            // Inicia o temporizador de parry
            parryTimer = parryWindow;
        }

        // Reduz o temporizador de parry ao longo do tempo
        parryTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D _other) //for up and down cast spell
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyGetsHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (pState.dashing || pState.healing || pState.cutscene) return;
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        openMap = Input.GetButton("Map");

        if (Input.GetButton("Cast"))
        {
            castTimer += Time.deltaTime;
        }
        else
        {
            castTimer = 0;
        }

        if (Input.GetButton("Heal"))
        {
            healTimer += Time.deltaTime;
        }
        else
        {
            healTimer = 0;
        }
    }

    void ToggleMap()
    {
        if (openMap)
        {
            UIManager.Instance.mapHandler.SetActive(true);
        }
        else
        {
            UIManager.Instance.mapHandler.SetActive(false);
        }
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        if (pState.healing) rb.velocity = new Vector2(0, 0);
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
           
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }

       
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0); 
        Stamina -= dashCost;
        if (Stamina < 0) Stamina = 0;
        StaminaBar.fillAmount = Stamina / MaxStamina;
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        pState.invincible = true;

        //If exit direction is upwards
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        //If exit direction requires horizontal movement
        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;
            Move();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.invincible = false;
        pState.cutscene = false;
    }

    [SerializeField] private GameObject slashEffectPrefab;
    void Attack()
    {
        if (Input.GetButtonDown("Attack"))
        {
            // Verifica se o jogador pode atacar (Stamina > 0) e se o tempo entre os ataques foi atingido
            if (Stamina > 0 && timeSinceAttack >= timeBetweenAttack)
            {
                // Reinicia o tempo de recarga da Stamina
                rechargeTime = 10f;

                // Outro código do método Attack() ...

                // Incrementa o tempo desde o último ataque
                timeSinceAttack = 0.7f;

                // Outro código do método Attack() ...
            }

            Stamina -= attackCost;
            if (Stamina < 0) Stamina = 0;
            StaminaBar.fillAmount = Stamina / MaxStamina;
        }

        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight,recoilXSpeed);
                InstantiateSlashEffect(SideAttackTransform.position, pState.lookingRight); // Instanciar SlashEffect
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up,recoilYSpeed);
                InstantiateSlashEffect(UpAttackTransform.position, true); // Instanciar SlashEffect
                if (CheckParrySuccess())
                {
                    InstantiateSlashEffect(UpAttackTransform.position, true); // Instanciar SlashEffect
                }
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                InstantiateSlashEffect(DownAttackTransform.position, true); // Instanciar SlashEffect
                if (CheckParrySuccess())
                {
                    InstantiateSlashEffect(DownAttackTransform.position, true); // Instanciar SlashEffect
                }
            }
        }

        void InstantiateSlashEffect(Vector3 position, bool isFacingRight)
        {
            GameObject slashEffect = Instantiate(slashEffectPrefab, position, Quaternion.identity);
            if (!isFacingRight)
            {
                // Se o jogador não estiver olhando para a direita, inverte a escala do SlashEffect
                slashEffect.transform.localScale = new Vector3(-1, 1, 1);
            }
            // Sincronizar a animação do SlashEffect com a animação de ataque do jogador
            // Exemplo: Se a animação de ataque do jogador for chamada "AttackAnimation", você pode fazer algo como:
            // slashEffect.GetComponent<Animator>().Play("AttackAnimation");
        }

        // Verifica se o jogador atacou no momento certo para realizar um parry
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            // Verifica se houve parry bem-sucedido
            if (CheckParrySuccess())
            {
                // Lógica para parry bem-sucedido
                Debug.Log("Parry bem-sucedido!");
            }
        }


    }

    void InstantiateSlashEffect(Vector2 position, bool lookingRight, bool isUpward)
    {
        GameObject slashEffectInstance = Instantiate(slashEffectPrefab, position, Quaternion.identity);

        // Ajuste a escala do efeito conforme a direção do jogador
        if (!lookingRight)
        {
            slashEffectInstance.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        // Verifique se o ataque é para cima ou para baixo e rotacione conforme necessário
        if (isUpward)
        {
            // Rotacione o eixo Z para cima (sentido anti-horário)
            slashEffectInstance.transform.Rotate(0f, 0f, 90f);
        }
        else
        {
            // Rotacione o eixo Z para baixo (sentido horário)
            slashEffectInstance.transform.Rotate(0f, 0f, -90f);
        }

        // Aqui você pode adicionar qualquer lógica adicional para sincronizar o efeito com a animação ou o ataque
    }

    bool CheckParrySuccess()
    {
        RaycastHit2D hitEnemy;
        RaycastHit2D hitObstacle;

        if (yAxis > 0)
        {
            hitEnemy = Physics2D.Raycast(UpAttackTransform.position, Vector2.up, UpAttackArea.y, enemyLayer);
            hitObstacle = Physics2D.Raycast(UpAttackTransform.position, Vector2.up, UpAttackArea.y, obstacleLayer);
        }
        else if (yAxis < 0)
        {
            hitEnemy = Physics2D.Raycast(DownAttackTransform.position, Vector2.down, DownAttackArea.y, enemyLayer);
            hitObstacle = Physics2D.Raycast(DownAttackTransform.position, Vector2.down, DownAttackArea.y, obstacleLayer);
        }
        else
        {
            hitEnemy = Physics2D.Raycast(SideAttackTransform.position, Vector2.right, SideAttackArea.x, enemyLayer);
            hitObstacle = Physics2D.Raycast(SideAttackTransform.position, Vector2.right, SideAttackArea.x, obstacleLayer);
        }

        if (hitEnemy.collider != null && parryTimer > 0)
        {
            return true;
        }

        if (hitObstacle.collider != null && parryTimer > 0)
        {
            InstantiateSlashEffect(hitObstacle.point, true, false); // SlashEffect como objeto de parry
            return true;
        }

        return false;
    }
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
           
            if (objectsToHit[i].GetComponent<Enemy>() !=null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyGetsHit (damage, _recoilDir, _recoilStrength);

               
            }
        }
    }
    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }
    public void TakeDamage(float _damage)
    {
       if (pState.alive)
        {
            Health -= Mathf.RoundToInt(_damage);
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }    
        }
    }
    public IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }
    IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.1f);
        canFlash = true;
    }
    void FlashWhileInvincible()
    {
        if (pState.invincible)
        {
            if (Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
    }
    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = _newTimeScale;
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActivateDeathScreen());

        yield return new WaitForSeconds(0.9f);
        
    }
    public void Respawned()
    {
        if (!pState.alive)
        {
            pState.alive = true;
            
            Health = maxHealth;
            anim.Play("Player_Idle");
        }
    }

   

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSecondsRealtime(_delay);
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }
    void Heal()
    {
        if (Input.GetButton("Heal") && healTimer > 0.05f && Health < maxHealth && Stamina > 0 && Grounded() && !pState.dashing)
        {
            pState.healing = true;
            anim.SetBool("Healing", true);

            //healing
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            //drain mana
            Stamina -= Time.deltaTime;
        }
        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }
   

    void CastSpell()
    {
        if (Input.GetButtonUp("Cast") && castTimer <= 0.05f && timeSinceCast >= timeBetweenCast && Stamina >= spellCost)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        if (Grounded())
        {
            //disable downspell if on the ground
            downSpellFireball.SetActive(false);
        }
        //if down spell is active, force player down until grounded
        if (downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
        }

        
    }
    IEnumerator CastCoroutine()
    {
        anim.SetBool("Casting", true);
        yield return new WaitForSeconds(0.15f);
        Stamina -= spellCost;
        if (Stamina < 0) Stamina = 0;
        StaminaBar.fillAmount = Stamina / MaxStamina;
        //side cast
        if (yAxis == 0 || (yAxis < 0 && Grounded()))
        {
            GameObject _fireBall = Instantiate(sideSpellFireball, SideAttackTransform.position, Quaternion.identity);

            //flip fireball
            if (pState.lookingRight)
            {
                _fireBall.transform.eulerAngles = Vector3.zero; // if facing right, fireball continues as per normal
            }
            else
            {
                _fireBall.transform.eulerAngles = new Vector2(_fireBall.transform.eulerAngles.x, 180);
                //if not facing right, rotate the fireball 180 deg
            }
            pState.recoilingX = true;
        }

        //up cast
        else if (yAxis > 0)
        {
            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;
        }

        //down cast
        else if (yAxis < 0 && !Grounded())
        {
            downSpellFireball.SetActive(true);
        }

        Stamina -= spellCost;
        yield return new WaitForSeconds(0.35f);
        anim.SetBool("Casting", false);
        pState.casting = false;
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            pState.jumping = true;
        }

        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
        {
            pState.jumping = true;

            airJumpCounter++;

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            pState.jumping = false;

            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while (Stamina < MaxStamina)
        {
            // Verifica se a Stamina não está zerada antes de recarregar
            if (Stamina > 0)
            {
                // Reinicia o tempo de recarga da Stamina quando o jogador ataca
                if (rechargeTime >= 0f)
                {
                    rechargeTime = 0f;
                }

                Stamina += chargeRate / 10f;
                if (Stamina > MaxStamina) Stamina = MaxStamina;
                StaminaBar.fillAmount = Stamina / MaxStamina;

                // Incrementa o tempo de recarga da Stamina
                rechargeTime += 0.1f;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}