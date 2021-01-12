using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle
}

public class Player : MonoBehaviour
{
    public PlayerState currentState;

    [Header("Player stats")]
    public float speed;

    [Header("Jumping")]
    public float jumpForce;
    public int extraJumpsMax;
    private int extraJumps;
    public bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask ground;
    private bool isJumping;
    public ParticleSystem dust;

    [Header("Slide")]
    public float slideForce;
    private float slideTime = 1.2f;
    public bool isSliding;
    public bool slideJump;
    [SerializeField]
    CapsuleCollider2D collider;

    [Header("Ledge")]
    public bool ledgeGrab;
    public bool ledgeJump;
    public Transform ledgeCheck;
    public LayerMask ledge;
    private bool climbingLedge;

    [Header("WallSlide")]
    public float wallSlideGravity = 0.3f;
    private bool wallSlide;
    public float wallJumpForce;
    public float wallJumpDuration;
    public bool wallJumping;

    [Header("Attacking")]
    public int numberOfClicks = 0;
    private float lastClickedTime = 0;
    public float maxComboDelay = 1.2f;
    public float dropSpeed = 3;
    public bool drop;

    [Header("Health & Shield")]
    public float maxHp;
    public float currentHp;
    public Slider hpBar;
    public float maxShield;
    public float currentShield;
    public float shieldDelay;
    private float shieldCounter;
    public float shieldReacharge;
    private float shieldRechargeRate = 0.001f;
    public Slider shieldBar;
    public float armor;
    private float armorRate = 0.004f;

    private Rigidbody2D myRigidbody;
    private Animator animator;
    private bool facingRight = true;
    private float moveInputX;
    private float moveY;
    public float additionalMoveX = 0f;



    void Start()
    {
        isJumping = false;
        extraJumps = extraJumpsMax;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        hpBar.maxValue = maxHp;
        currentHp = maxHp;
        hpBar.value = currentHp;
        currentShield = maxShield;
        shieldBar.maxValue = maxShield;
        shieldBar.value = currentShield;
        shieldCounter = 0;
    }

    void FixedUpdate()
    {
        if (wallJumping) StartCoroutine(wallJump());
        moveInputX = 0;
        if (!climbingLedge && !wallJumping && !ledgeGrab) moveInputX = Input.GetAxis("Horizontal");
        if (drop) moveInputX = 0;
        if (!climbingLedge && !isSliding) { myRigidbody.velocity = new Vector2(moveInputX * speed + additionalMoveX, myRigidbody.velocity.y); }
        if (currentState == PlayerState.attack && isGrounded) myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        /*if (Input.GetButton("Crouch")) { animator.SetBool("crouch", true); }
        else if (Input.GetButtonUp("Crouch")) { animator.SetBool("crouch", false); }*/
        if (facingRight == false && moveInputX > 0 && !isSliding)
        {
            Flip();
        }
        else if (facingRight == true && moveInputX < 0 && !isSliding)
        {
            Flip();
        }  
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
        if (!(currentState == PlayerState.attack) && !isSliding)
        {
            Jump();

            if (isGrounded)
            {
                Slide();
            }
        }

        Attack();
        UpdateAnimation();
        Shield();
        Ledge();
    }

    void UpdateAnimation()
    {
        if (Mathf.Abs(moveInputX) > 0.01 && !animator.GetBool("running")) { animator.SetBool("running", true); }
        else if (Mathf.Abs(moveInputX) < 0.01 && animator.GetBool("running")) { animator.SetBool("running", false); }
        moveY = myRigidbody.velocity.y;
        animator.SetFloat("moveY", Mathf.Round(moveY));
        if (!isJumping || ledgeJump)
        {
            animator.SetBool("jumping", false);
        }
        else
        {
            animator.SetBool("jumping", true);
            isJumping = false;
        }
        if (isGrounded) animator.SetBool("isGrounded", true);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
        if (isGrounded) CreateDust();
    }

    public void Ledge()
    {
        ledgeGrab = Physics2D.OverlapCircle(ledgeCheck.position, checkRadius, ledge);
        bool touchingWall = Physics2D.OverlapCircle(ledgeCheck.position, checkRadius, ground);
        if (ledgeJump)
        {
            wallSlide = false;
            if (!touchingWall) ledgeJump = false;
            myRigidbody.gravityScale = 1;
            StopSliding();
        }else
        {
            animator.SetBool("ledgeJump", false);
            if (ledgeGrab && !drop)
            {
                wallSlide = false;
                animator.SetBool("ledgeGrab", true);
                myRigidbody.velocity = Vector2.zero;
                myRigidbody.gravityScale = 0;
                if (extraJumps == 0) extraJumps = 1;
                animator.SetBool("isGrounded", false);

                if (Input.GetButtonDown("Jump") && Input.GetAxis("Vertical") >= 0) {
                    ledgeJump = true;
                    animator.SetBool("ledgeGrab", false);
                    myRigidbody.gravityScale = 1;
                    animator.SetBool("jumping", false);
                    animator.SetBool("ledgeJump", true);
                    StartCoroutine(climbeLedge());
                }

                else if(Input.GetButtonDown("Jump") && Input.GetAxis("Vertical") < 0)
                {
                    ledgeJump = true;
                    animator.SetBool("ledgeGrab", false);
                    myRigidbody.gravityScale = 1;
                    animator.SetBool("jumping", false);
                    animator.SetBool("ledgeFall", true);
                }
            }
            else
            {
                animator.SetBool("ledgeGrab", false);
            }
        }
    }

    IEnumerator climbeLedge()
    {
        float move = 5f;
        if (!facingRight) move = -move;
        climbingLedge = true;
        myRigidbody.velocity = Vector2.up * jumpForce * 1.5f;
        yield return new WaitForSeconds(0.3f);
        myRigidbody.velocity = Vector2.right * move;
        yield return new WaitForSeconds(0.2f);
        climbingLedge = false;
    }

    private void Jump()
    {
        wallSlide = Physics2D.OverlapCircle(ledgeCheck.position, checkRadius, ground);
        if(wallSlide)
        {
            StopSliding();
        }
        if (isGrounded)
        {
            extraJumps = extraJumpsMax;
            animator.SetBool("ledgeFall", false);
            wallSlide = false;
            myRigidbody.gravityScale = 1; ///To be changed
        } else
        {
            animator.SetBool("isGrounded", false);
        }
        if(wallSlide && !drop && !ledgeJump)
        {
            animator.SetBool("wallSlide", true);
            if(moveY<-0.1f) myRigidbody.gravityScale = wallSlideGravity;
            wallJumping = false;
            StopAllCoroutines();
            additionalMoveX = 0;
            isSliding = false;
            animator.SetBool("slide", false);
        }
        else
        {
            animator.SetBool("wallSlide", false);
            if (myRigidbody.gravityScale == wallSlideGravity) myRigidbody.gravityScale = 1;
        }
        if (wallSlide && !ledgeGrab && Input.GetButtonDown("Jump") && !wallJumping)
        {
            myRigidbody.velocity = Vector2.up * jumpForce;
            wallJumping = true;
            if (myRigidbody.gravityScale == wallSlideGravity) myRigidbody.gravityScale = 1;
            isJumping = true;
            Flip();
            CreateDust();
        }
        else if (Input.GetButtonDown("Jump") && extraJumps > 0 && !drop)
        {
            myRigidbody.velocity = Vector2.up * jumpForce;
            extraJumps--;
            isJumping = true;
            if (!ledgeGrab && !ledgeJump) CreateDust();
        }
    }

    private void Slide()
    {
        if (Mathf.Abs(moveInputX) > 0.5 && Input.GetButtonDown("slide"))
        {
            myRigidbody.velocity = new Vector2(moveInputX * jumpForce, 0);
            StartCoroutine(SlideCO(moveInputX > 0));
        }
    }

    private void StopSliding()
    {
        animator.SetBool("slide", false);
        additionalMoveX = 0;
        isSliding = false;
        slideJump = false;
        collider.size = new Vector2(0.8f, 1.8f);
        collider.offset = new Vector2(0, -0.17f);
    }

    IEnumerator SlideCO(bool headingRight)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        isSliding = true;
        if(headingRight)
        {
            additionalMoveX = slideForce;
        }
        else
        {
            additionalMoveX = -slideForce;
        }

        animator.SetBool("slide", true);
        collider.size = new Vector2(0.8f, 1);
        collider.offset = new Vector2(0, -0.57f);
        do
        {
            //Slow down
            if(headingRight)
            {
                additionalMoveX -= 0.1f;
            }
            else
            {
                additionalMoveX += 0.1f;
            }
            //Stop
            if(!isGrounded)
            {
                animator.SetBool("slide", false);
            }
            else
            {
                animator.SetBool("slide", true);
            }
            if (sw.ElapsedMilliseconds > slideTime * 1000 || !isSliding)
            {
                break;
            }
            if (sw.ElapsedMilliseconds > slideTime * 1000 * 0.8f)
            {
                TryToSlideJump(headingRight);
            }
            yield return new WaitForSeconds(0.1f);
        } while (isSliding);

        StopSliding();

        sw.Stop();
    }

    void TryToSlideJump(bool headingRight)
    {
        if(Input.GetButtonDown("Jump"))
        {
            StartCoroutine(SlideJumpCO(headingRight));
        }
    }

    IEnumerator SlideJumpCO(bool headingRight)
    {
        if(headingRight)
        {
            additionalMoveX = slideForce;
        }
        else
        {
            additionalMoveX = -slideForce;
        }

        Stopwatch sw = new Stopwatch();
        sw.Start();
        /*
        do
        {
            //Slow down
            if (headingRight && additionalMoveX > 0)
            {
                additionalMoveX -= 0.1f;
            }
            else if (additionalMoveX < 0)
            {
                additionalMoveX += 0.1f;
            }
            else
            {
                additionalMoveX = 0;
                break;
            }

            yield return new WaitForSeconds(0.1f);
        } while (sw.ElapsedMilliseconds < slideTime * 1000);
        sw.Reset();
        */
        yield return null;
        additionalMoveX = 0;
    }

    IEnumerator wallJump()
    {
        int steps = 10;
        float currentWallJumpForce = -wallJumpForce;

        int right = -1;
        if (!facingRight) right = 1;

        for (int i = 0; i < steps; i++)
        {
            additionalMoveX = currentWallJumpForce * right;
            yield return new WaitForSeconds(wallJumpDuration/steps);
            if (i == 4) wallJumping = false;
            currentWallJumpForce += wallJumpForce / steps;
            if (Mathf.Abs(currentWallJumpForce) < 0.1) currentWallJumpForce = 0;
            if (Mathf.Abs(myRigidbody.velocity.x) < 0.5) currentWallJumpForce = 0;
        }

        additionalMoveX = 0;
    }

    public void Attack()
    {
        //Timer
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numberOfClicks = 0;
        }
        //Ground
        if (Input.GetButtonDown("attack") && isGrounded && !isJumping && !isSliding)
        {
            currentState = PlayerState.attack;
            lastClickedTime = Time.time;
            numberOfClicks++;
            if (numberOfClicks == 1)
            {
                animator.SetBool("attack1", true);
            }
            numberOfClicks = Mathf.Clamp(numberOfClicks, 0, 3);
        }
        //Jump
        if (Input.GetButtonDown("attack") && !isGrounded && !wallSlide && !ledgeGrab && !drop)
        {
            currentState = PlayerState.attack;
            lastClickedTime = Time.time;
            numberOfClicks++;
            if (numberOfClicks == 1)
            {
                animator.SetBool("jumpAttack1", true);
                myRigidbody.gravityScale = 1;
            }
            numberOfClicks = Mathf.Clamp(numberOfClicks, 0, 3);
        }
        if(isGrounded)
        {
            if (animator.GetBool("jumpAttack1") || animator.GetBool("jumpAttack2"))
            {
                numberOfClicks = 0;
                currentState = PlayerState.idle;
            }
            animator.SetBool("jumpAttack1", false);
            animator.SetBool("jumpAttack2", false);
        }
    }

    #region Attack functions
    public void attack1()
    {
        if (numberOfClicks >= 2)
        {
            animator.SetBool("attack2", true);
        }
        else
        {
            animator.SetBool("attack1", false);
            numberOfClicks = 0;
            currentState = PlayerState.idle;
        }
    }

    public void attack2()
    {
        if (numberOfClicks >= 3)
        {
            animator.SetBool("attack3", true);
        }
        else
        {
            animator.SetBool("attack2", false);
            numberOfClicks = 0;
            currentState = PlayerState.idle;
        }
    }

    public void attack3()
    {
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
        numberOfClicks = 0;
        currentState = PlayerState.idle;
    }

    public void jumpAttack1()
    {
        if (numberOfClicks >= 2 && !isGrounded)
        {
            animator.SetBool("jumpAttack2", true);
        }
        else
        {
            animator.SetBool("jumpAttack1", false);
            numberOfClicks = 0;
            currentState = PlayerState.idle;
            myRigidbody.gravityScale = 1;
        }
    }

    public void jumpAttack2()
    {
        if (numberOfClicks >= 3 && !isGrounded)
        {
            animator.SetBool("jumpAttack3", true);
            myRigidbody.gravityScale = dropSpeed;
            drop = true;
            currentState = PlayerState.idle;
        }
        else
        {
            animator.SetBool("jumpAttack2", false);
            animator.SetBool("jumpAttack1", false);
            numberOfClicks = 0;
            currentState = PlayerState.idle;
            myRigidbody.gravityScale = 1;
        }
    }

    public void jumpaAttack3()
    {
        animator.SetBool("jumpAttack1", false);
        animator.SetBool("jumpAttack2", false);
        animator.SetBool("jumpAttack3", false);
        numberOfClicks = 0;
        currentState = PlayerState.idle;
        myRigidbody.gravityScale = 1;
        drop = false;
    }
    #endregion

    public void Shield()
    {
        if(shieldCounter <= 0 && currentShield<maxShield)
        {
            currentShield += shieldReacharge * shieldRechargeRate;
            if (currentShield > maxShield) currentShield = maxShield;
        } else if(shieldCounter > 0) shieldCounter -= Time.deltaTime;
        shieldBar.value = currentShield;
    }

    public void takeDMG(float dmg)
    {
        dmg -= dmg * armor * armorRate;
        if (currentShield > dmg) currentShield -= dmg;
        else if (currentShield < dmg && currentShield > 0)
        {
            currentHp = currentHp + currentShield - dmg;
            currentShield = 0;
        }
        else if(currentShield == 0)
        {
            currentHp -= dmg;
        }
        if (currentHp <= 0) death();

        shieldCounter = shieldDelay;
    }

    public void death()
    {
        //TBD
        this.gameObject.SetActive(false);
    }

    void CreateDust() { dust.Play(); }
}
