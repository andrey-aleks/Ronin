using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy, IDamageable
{
    [SerializeField]
    private Vector3 _currentTarget;
    private Vector3 localScale;
    private Animator _anim;
    private SpriteRenderer _enemySprite;
    private Rigidbody2D rigid;

    [SerializeField]
    private Transform healthBar;

    [SerializeField]
    public int Health { get; set; }
    [SerializeField]
    private float hScale;
    [SerializeField]
    private float timer;
    private float lastPos;

    [SerializeField]
    private bool isWalled;
    private bool isGrounded;
    [SerializeField]
    private bool isTriggered;
    [SerializeField]
    private bool isInCombat;
    [SerializeField]
    private bool lastTrigger;
    [SerializeField]
    private bool lastCombat;
    [SerializeField]
    private bool isWaiting;
    [SerializeField]
    private bool isWalking;
    private bool _isDead;
    public Vector3 player;

    private void Start()
    {
        lastPos = transform.position.x;
        rigid = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _enemySprite = GetComponentInChildren<SpriteRenderer>();
        isFacingRight = true;
        isInCombat = false;
        Health = base.health;
        localScale = healthBar.localScale;
        _isDead = false;
    }

    public override void Update()
    {
        if (_isDead)
        {
            Death();
        }
        else if (GameManager.Instance.finishCheck)
        {
            Destroy(gameObject);
        }
        else
        {
            player = pointE.position;
            Movement();
            Attack();
            Jump();
            CheckFacing();
            UpdateAnimations();
            localScale.x = hScale;
            healthBar.localScale = localScale;
            lastCombat = isInCombat;
            lastTrigger = isTriggered;
        }

    }

    private void FixedUpdate()
    {
        if (_isDead)
        {
            return;
        }
        CheckSurroundings();
    }

    IEnumerator Waiting()
    {
        //isWaiting = true;
        yield return new WaitForSeconds(1.0f);
        _anim.SetTrigger("Idle");
        //isWaiting = false;
    }

    IEnumerator OutOfCombat()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2.0f);
        _currentTarget = pointA.position;
        isWaiting = false;
    }

    void Movement()
    {
        if (!isTriggered && !isInCombat)
        {
            if (transform.position.x == pointA.position.x)
            {
                _currentTarget = pointB.position;
                //_anim.SetTrigger("Idle");
            }
            else if ((transform.position.x == pointB.position.x) && isFacingRight)
            {
                _currentTarget = pointA.position;
                //_anim.SetTrigger("Idle");
            }
            else if (lastTrigger || lastCombat)
            {
                StartCoroutine(OutOfCombat());
                //_currentTarget = pointA.position;
                //_anim.SetTrigger("Idle");
                //StartCoroutine("OutOfCombat");
            }
        }
        else if (!isInCombat)
        {
            _currentTarget = pointE.position;
        }

        if (!isInCombat && !isWaiting)
        {
            transform.position = Vector3.MoveTowards(transform.position, _currentTarget, speed * Time.deltaTime);
            //rigid.velocity = new Vector2(movDirect*speed, rigid.velocity.y);
        }

        //if (transform.position.x < _currentTarget.x)
        //movDirect = 1f;
        //if (transform.position.x > _currentTarget.x)
        //movDirect = -1f;

        if ((transform.position.x != lastPos) && !isInCombat && !isWaiting)
            isWalking = true;
        else
            isWalking = false;
    }

    public void Damage()
    {
        Debug.Log("Hit: ");
        Health = Health - 1;
        hScale = Health * 1f / base.health * 1f;
        //_anim.SetTrigger("Hit");
        if (Health <= 0)
        {
            _isDead = true;
            _anim.SetTrigger("Death");
        }
    }

    public void Death()
    {
        GameManager.Instance.AddKilledEnemy(1);
        Destroy(gameObject);
    }

    private void Attack()
    {
        if (isInCombat)
        {
            timer += Time.deltaTime;
            if (timer < 0.6f) return;
            _anim.SetTrigger("isAttacking");
            timer = 0;
        }
    }

    void Jump()
    {
        if (isWalled && _currentTarget.y > transform.position.y)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 3);
        }
    }

    public override void Flip()
    {
        transform.Rotate(0.0f, 180.0f, 0.0f);
        playerCheckDistance = playerCheckDistance * -1;
        wallCheckDistance = wallCheckDistance * -1;
        combatCheckDistance = combatCheckDistance * -1;
        isFacingRight = !isFacingRight;
    }

    private void CheckSurroundings()
    {
        isTriggered = Physics2D.Raycast(playerCheck.position, Vector2.right, playerCheckDistance, 1 << 9);
        isWalled = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, 1 << 8);
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, 1 << 8);
        isInCombat = Physics2D.Raycast(wallCheck.position, Vector2.right, combatCheckDistance, 1 << 9);
    }

    private void CheckFacing()
    {
        if ((_currentTarget.x < transform.position.x) && (isFacingRight))
            Flip();
        if ((_currentTarget.x > transform.position.x) && (!isFacingRight))
            Flip();
    }

    private void UpdateAnimations()
    {
        _anim.SetBool("isWalking", isWalking);
        _anim.SetBool("isWaiting", isWaiting);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + playerCheckDistance, playerCheck.position.y, playerCheck.position.z));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - 0.3f, groundCheck.position.z));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DeathZoneCheck(collision);
    }

    private void DeathZoneCheck(Collider2D collision)
    {
        if (collision.tag == "DeathZone")
        {
            _isDead = true;
        }
    }

}
