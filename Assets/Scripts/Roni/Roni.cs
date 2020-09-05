using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityStandardAssets.CrossPlatformInput;

public class Roni : MonoBehaviour, IDamageable
{
    private Rigidbody2D rigid;
    private Animator anim;
    private Sound runSound;

    public int Health { get; set; }
    [SerializeField]
    private int baseHealth;

    [SerializeField]
    private float jumpForce = 4.0f;
    private float variableJumpHeighMultiplier = 0.5f;
    [SerializeField]
    private float wallSlideSpeed = 0.5f;
    [SerializeField]
    private float movementSpeed = 1f;
    [SerializeField]
    private float movementForceInAir = 0.8f;
    [SerializeField]
    private float horizontalInput;
    private float wallCheckDistance = 0.15f;
    private float wallHopForce = 5;
    [SerializeField]
    private float wallJumpForce = 5;
    private float hScale = 1.0f;
    private float _spikesForce = 2.8f;
    [SerializeField]
    // private float gravity = 20.0f;
    private float timer;

    [SerializeField]
    private int amountOfJumps = 1;
    [SerializeField]
    private int jumpsLeft = 1;
    private int facingDirection = 1;

    private bool isFacingRight = true;
    [SerializeField]
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canJump;
    private bool attackReloading;
    private bool deathWaiting;
    private bool _dead;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform healthBar;


    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;
    private Vector3 localScale;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        // wallJumpDirection.Normalize();

        Health = baseHealth;
        localScale = healthBar.localScale;
        runSound = Array.Find(AudioManager.Instance.sounds, sound => sound.name == "heroRun");
        _dead = false;
        horizontalInput = 0;
        rigid.velocity = new Vector2(0, 0);
        CrossPlatformInputManager.SetButtonUp("Left_Button");
        CrossPlatformInputManager.SetButtonUp("Right_Button");
        CrossPlatformInputManager.SetButtonUp("Attack");
        CrossPlatformInputManager.SetButtonUp("Jump");



    }

    void Update()
    {
        if (GameManager.Instance.finishCheck) {
            anim.SetBool("isWalking", false);
            isWalking = false;
            horizontalInput = 0;
            return;
        }
        if (_dead)
        {
            Death();
        }
        else
        {
            UIManager.Instance.StartUI();
            CheckInput();
            CheckMovementDirection();
            CheckCanJump();
            CheckIsWallSliding();
            UpdateAnimations();
            WalkSound();
        }
    }

    private void FixedUpdate()
    {
        if (_dead)
        {
            return;
        }
        else
        {
            ApplyMovement();
            CheckSurroundings();
            localScale.x = hScale;
            healthBar.localScale = localScale;
            // rigid.AddForce(new Vector3(0, -gravity * rigid.mass, 0), ForceMode2D.Impulse);
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, 1 << 8);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, 1 << 8);
    }

    private void CheckIsWallSliding()
    {
        if (isTouchingWall && !isGrounded && (rigid.velocity.y < 0) && (horizontalInput != 0))
        {
            isWallSliding = true;
        }
        else isWallSliding = false;
    }

    private void CheckCanJump()
    {
        if ((isGrounded && rigid.velocity.y <= 0) || isWallSliding)
        {
            jumpsLeft = amountOfJumps;
        }

        if (jumpsLeft <= 0)
            canJump = false;
        else
            canJump = true;
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && horizontalInput < 0)
            Flip();
        else if (!isFacingRight && horizontalInput > 0)
            Flip();

        if ((rigid.velocity.x != 0) && isGrounded)
            isWalking = true;
        else
            isWalking = false;
    }

    private void WalkSound()
    {
        if (isWalking)
        {
            if (!runSound.source.isPlaying)
            {
                AudioManager.Instance.Play("heroRun");
            }
        }
        else
        {
            runSound.source.Stop();
        }
    }

    private void CheckInput()
    {
        {
            //only one of these vars of inputs must be umcommented

            // for 2 move buttons (mobile)

            if (CrossPlatformInputManager.GetButton("Left_Button"))
            {
                horizontalInput = -1;
            }
            else if (CrossPlatformInputManager.GetButton("Right_Button"))
            {
                horizontalInput = 1;
            }
            else horizontalInput = 0;



            // horizontalInput = Input.GetAxisRaw("Horizontal"); // for the PC testing


            // horizontalInput = CrossPlatformInputManager.GetAxisRaw("Horizontal"); // for the joystick movement (mobile)
        }
        if (Input.GetButtonDown("Jump") || CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.F) || CrossPlatformInputManager.GetButtonDown("Attack"))
        {
            Attack();
        }
    }

    void ApplyMovement()
    {
        if (isGrounded)
        {
            rigid.velocity = new Vector2(movementSpeed * horizontalInput, rigid.velocity.y);
        }

        else if (!isWallSliding && (horizontalInput != 0))
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * horizontalInput, 0);
            // rigid.AddForce(forceToAdd, ForceMode2D.Impulse);
            rigid.velocity = new Vector2(horizontalInput * movementSpeed, rigid.velocity.y);
            // 


            if (Mathf.Abs(rigid.velocity.x) > movementSpeed)
            {
                rigid.velocity = new Vector2(movementSpeed * horizontalInput, rigid.velocity.y);
            }
        }
        if (isWallSliding)
        {
            if (rigid.velocity.y < -wallSlideSpeed)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, -wallSlideSpeed);
            }
        }




    }

    public void Damage()
    {
        Debug.Log("Damage ");
        anim.SetTrigger("Hit");
        Health = Health - 1;
        hScale = Health * 1f / baseHealth * 1f;
        if (Health <= 0)
        {
            _dead = true;
            deathWaiting = true;
        }
    }

    public void Death()
    {
        UIManager.Instance.DeathUI();
        StartCoroutine(DeathWait());
        if (!deathWaiting)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    IEnumerator DeathWait()
    {
        yield return new WaitForSeconds(2.0f);
        deathWaiting = false;
    }

    void Jump()
    {
        if (canJump && !isWallSliding)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
            jumpsLeft--;
            AudioManager.Instance.Play("heroJump");
        }
        /*
        else if (isWallSliding && horizontalInput == 0 && canJump)
        {
            isWallSliding = false;
            jumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rigid.AddForce(forceToAdd, ForceMode2D.Impulse);
            Flip();
            AudioManager.Instance.Play("heroJump");
        }
        */
        else if ((isWallSliding || isTouchingWall) && horizontalInput != 0 && canJump)
        {
            isWallSliding = false;
            jumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * horizontalInput, wallJumpForce * wallJumpDirection.y);
            rigid.velocity = Vector2.zero;
            rigid.AddForce(forceToAdd, ForceMode2D.Impulse);
            AudioManager.Instance.Play("heroJump");
        }

    }


    public void Attack()
    {
        if (isGrounded)
        {
            if (!attackReloading)
            {
                StartCoroutine(Attacking());
                AudioManager.Instance.Play("heroAttack");
                attackReloading = true;
            }
        }
    }

    IEnumerator Attacking()
    {
        anim.SetTrigger("isAttacking");
        yield return new WaitForSeconds(.6f);
        attackReloading = false;
    }

    private void Flip()
    {
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
        wallCheckDistance *= -1;
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetFloat("yVelocity", rigid.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_dead)
        {
            return;
        }
        DeathZoneCheck(collider);
        ChestCheck(collider);
        HPotionCheck(collider);
        SpikesCheck(collider);
    }

    public void DeathZoneCheck(Collider2D collider)
    {
        if (collider.tag == "DeathZone")
        {
            _dead = true;
            deathWaiting = true;
        }
    }

    public void ChestCheck(Collider2D collider)
    {
        if (collider.tag == "Skull")
        {
            Skull skull = collider.GetComponent<Skull>();
            GameManager.Instance.AddKilledEnemy(skull.Value);
            Destroy(collider.gameObject);
        }
    }

    public void HPotionCheck(Collider2D collider)
    {
        if (collider.tag == "Potion")
        {
            HPotion hPotion = collider.GetComponent<HPotion>();
            if (hPotion)
            {
                if (Health < baseHealth)
                {
                    Health += hPotion.Value;
                    hScale = Health * 1f / baseHealth * 1f;
                }
                else
                {
                    GameManager.Instance.AddKilledEnemy(hPotion.Value);
                }

                Destroy(collider.gameObject);
                AudioManager.Instance.Play("pickup");
            }
        }
    }

    public void SpikesCheck(Collider2D collider)
    {
        if (collider.tag == "Spikes")
        {
            Damage();
            AudioManager.Instance.Play("spikes");
            rigid.velocity = Vector3.zero;
            Vector2 direction = (transform.position - collider.transform.position).normalized;
            rigid.AddForce(direction * _spikesForce, ForceMode2D.Impulse);

        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, new Vector3(groundCheck.position.x, wallCheck.position.y - 0.3f, wallCheck.position.z));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

}
