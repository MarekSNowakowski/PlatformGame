using System.Collections;
using System.Diagnostics;
using UnityEngine;

public enum PlayerState
{
    walk,
    attack,
    idle,
    drop,
    jump,
    ledgeGrab,
    ledgeJump,
    wallSlide,
    wallJump,
    slide,
    stagger
}

public class PlayerController : MonoBehaviour
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
    public DustHandler dustHandler;

    [Header("Slide")]
    public float slideForce;
    public float slideTime = 1.2f;
    public bool isSliding;
    [SerializeField]
    private CapsuleCollider2D playerCollider;
    private const float slideJumpTreshold = 0.15f;

    [Header("Ledge")]
    public bool ledgeGrab;
    public bool ledgeJump;
    public Transform ledgeCheck;
    public LayerMask ledge;
    public float ledgeCheckRadius;
    private bool climbingLedge;

    [Header("WallSlide")]
    public float wallSlideGravity = 0.3f;
    private bool wallSlide;
    public float wallJumpForce;
    public float wallJumpDuration;
    public bool wallJumping;
    public float additionalMoveX = 0f;

    [Header("Attacking")]
    public int numberOfClicks = 0;
    private float lastClickedTime = 0;
    public float maxComboDelay = 1.2f;
    public float dropSpeed = 3;
    public bool drop;
    
    private Rigidbody2D myRigidbody;
    private Animator animator;
    [SerializeField]
    private bool facingRight = true;
    private float moveInputX;
    private float moveY;
    
    private GatheredInput currentInput;

    [SerializeField] private AttackHandler attackHandler1;
    [SerializeField] private AttackHandler attackHandler2;
    [SerializeField] private AttackHandler attackHandler3;

    private bool isActive = true;
    private float safetyTimer = 0f;

    private void Start()
    {
        isJumping = false;
        extraJumps = extraJumpsMax;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (facingRight != animator.GetBool("facingRight"))
        {
            Flip();
        }

        if (animator.GetBool("ledgeGrab"))
        {
            myRigidbody.gravityScale = 0;
        }
    }

    private void FixedUpdate()
    {
        // if (animator.GetBool("jumpAttack3"))
        // {
        //     myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        //     currentInput.movementInput = new Vector2(0, currentInput.movementInput.y);
        //     moveInputX = 0;
        // }
    }

    public void UpdatePlayer(GatheredInput recivedInput)
    {
        if (isActive)
        {
            currentInput = recivedInput;
            HandleMoveInput();
            HandleFlipping();
        }
        
        HandleJumpAndSlide();
        
        if (isActive)
        {
            Attack();
            Ledge();
        }
        
        UpdateAnimation();

        SafetyEndGameCheck();
    }

    public void OnDeath()
    {
        ledgeGrab = false;
        additionalMoveX = 0;
        myRigidbody.gravityScale = 1;
        animator.SetBool("ledgeGrab", false);
        isActive = false;
    }

    /// <summary>
    /// In case we die mid air and game is waiting too long for us to fall :) (or animator breaks)
    /// </summary>
    private void SafetyEndGameCheck()
    {
        if (!isActive && !isGrounded)
        {
            safetyTimer += Time.deltaTime;
            if (safetyTimer > 5f)
            {
                animator.SetBool("death", true);
                animator.SetBool("isGrounded", true);
            }
        }
    }

    private void HandleMoveInput()
    {
        moveInputX = 0;
        
        // if (animator.GetBool("jumpAttack3"))
        // {
        //     myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        //     currentInput.movementInput = new Vector2(0, currentInput.movementInput.y);
        //     moveInputX = 0;
        // }
        
        if (!climbingLedge && !wallJumping && !ledgeGrab && !drop && !animator.GetBool("jumpAttack3"))
        {
            moveInputX = currentInput.movementInput.x;
            
            if (currentState == PlayerState.idle && Mathf.Abs(moveInputX) > Mathf.Epsilon)
            {
                currentState = PlayerState.walk;
            }
            else if(currentState == PlayerState.walk && Mathf.Abs(moveInputX) < Mathf.Epsilon)
            {
                currentState = PlayerState.idle;
            }
        }

        if (ledgeGrab)
        {
            drop = false;
        }

        if (drop)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }
        
        if (!climbingLedge && !isSliding)
        {
            myRigidbody.velocity = new Vector2(moveInputX * speed + additionalMoveX, myRigidbody.velocity.y);
        }
    }
    
    private void HandleFlipping()
    {
        if (facingRight == false && moveInputX > 0 && !isSliding)
        {
            animator.SetBool("facingRight", true);
        }
        else if (facingRight == true && moveInputX < 0 && !isSliding)
        {
            animator.SetBool("facingRight", false);
        }
    }
    
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
        if (isGrounded) CreateDust();
    }
    
    private void HandleJumpAndSlide()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
        if (currentState != PlayerState.attack && !isSliding)
        {
            HandleJumping();

            if (isGrounded)
            {
                drop = false;
                Slide();
            }
        }
    }

    private void Attack()
    {
        if (currentState == PlayerState.attack && isGrounded) myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);

        //Timer
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numberOfClicks = 0;
        }
        //Ground
        if (currentInput.attack && isGrounded && !isJumping && !isSliding)
        {
            currentState = PlayerState.attack;
            lastClickedTime = Time.time;
            numberOfClicks++;
            if (numberOfClicks == 1)
            {
                animator.SetBool("attack1", true);
                attackHandler1.StartAttack();
            }
            numberOfClicks = Mathf.Clamp(numberOfClicks, 0, 3);
        }
        //Jump
        if (currentInput.attack && !isGrounded && !wallSlide && !ledgeGrab && !drop)
        {
            currentState = PlayerState.attack;
            lastClickedTime = Time.time;
            numberOfClicks++;
            if (numberOfClicks == 1)
            {
                animator.SetBool("jumpAttack1", true);
                attackHandler1.StartAttack();
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

    private void UpdateAnimation()
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

    private void Ledge()
    {
        ledgeGrab = Physics2D.OverlapCircle(ledgeCheck.position, ledgeCheckRadius, ledge);
        bool touchingWall = Physics2D.OverlapCircle(ledgeCheck.position, ledgeCheckRadius, ground);
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
                currentState = PlayerState.ledgeGrab;
                animator.SetBool("ledgeGrab", true);
                myRigidbody.velocity = Vector2.zero;
                myRigidbody.gravityScale = 0;
                if (extraJumps == 0) extraJumps = 1;
                animator.SetBool("isGrounded", false);

                if (currentInput.jump && currentInput.movementInput.y >= 0) {
                    ledgeJump = true;
                    animator.SetBool("ledgeGrab", false);
                    myRigidbody.gravityScale = 1;
                    animator.SetBool("jumping", false);
                    animator.SetBool("ledgeJump", true);
                    currentState = PlayerState.ledgeJump;
                    StartCoroutine(ClimbeLedgeCO());
                }

                else if(currentInput.jump && currentInput.movementInput.y  < 0)
                {
                    ledgeJump = true;
                    currentState = PlayerState.ledgeJump;
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

    private IEnumerator ClimbeLedgeCO()
    {
        float move = 5f;
        if (!facingRight) move = -move;
        climbingLedge = true;
        myRigidbody.velocity = Vector2.up * (jumpForce * 1.5f);
        yield return new WaitForSeconds(0.3f);
        myRigidbody.velocity = Vector2.right * move;
        yield return new WaitForSeconds(0.2f);
        climbingLedge = false;
        currentState = PlayerState.idle;
    }

    private void HandleJumping()
    {
        wallSlide = Physics2D.OverlapCircle(ledgeCheck.position, checkRadius, ground);
        if (wallJumping) StartCoroutine(WallJumpCO());
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
            currentState = PlayerState.wallSlide;
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
        if (wallSlide && !ledgeGrab && currentInput.jump && !wallJumping)
        {
            currentState = PlayerState.wallJump;
            myRigidbody.velocity = Vector2.up * jumpForce;
            wallJumping = true;
            if (myRigidbody.gravityScale == wallSlideGravity) myRigidbody.gravityScale = 1;
            isJumping = true;
            animator.SetBool("facingRight", !facingRight);
            CreateDust();
        }
        else if (currentInput.jump && extraJumps > 0 && !drop)
        {
            Jump();
        }
    }

    private void Jump()
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
        extraJumps--;
        isJumping = true;
        currentState = PlayerState.jump;
        if (!ledgeGrab && !ledgeJump) CreateDust();
    }

    private void Slide()
    {
        if (Mathf.Abs(moveInputX) > 0.5 && currentInput.slide)
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
        playerCollider.size = new Vector2(0.8f, 1.8f);
        playerCollider.offset = new Vector2(0, -0.17f);
    }

    private IEnumerator SlideCO(bool headingRight)
    {
        bool slideJump = false;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        isSliding = true;
        currentState = PlayerState.slide;
        if(headingRight)
        {
            additionalMoveX = slideForce;
        }
        else
        {
            additionalMoveX = -slideForce;
        }

        animator.SetBool("slide", true);
        playerCollider.size = new Vector2(0.8f, 1);
        playerCollider.offset = new Vector2(0, -0.57f);
        do
        {
            if (currentInput.jump)
            {
                slideJump = true;
            }
            
            //Slow down
            if(headingRight)
            {
                additionalMoveX = slideForce - (slideForce * (sw.ElapsedMilliseconds * 1000 / slideTime));
            }
            else
            {
                additionalMoveX = -slideForce + (slideForce * (sw.ElapsedMilliseconds * 1000 / slideTime));
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
            if (sw.ElapsedMilliseconds > slideTime * 1000f || !isSliding)
            {
                break;
            }

            yield return null;
        } while (isSliding);

        StopSliding();
        sw.Stop();

        if (slideJump)
        {
            // Slide jumping
            Jump();
            
            yield return  null;
            
            if(headingRight)
            {
                additionalMoveX = slideForce;
            }
            else
            {
                additionalMoveX = -slideForce;
            }
            
            yield return new WaitForSeconds(slideTime);
            extraJumps--;
            
            additionalMoveX = 0;
        }
    }

    public void Stagger(float force, float time)
    {
        if (gameObject.activeInHierarchy)
        {
            additionalMoveX = force;
            StartCoroutine(StaggerCO(time));
            currentState = PlayerState.stagger;
        }
    }

    private IEnumerator WallJumpCO()
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

    private IEnumerator StaggerCO(float time)
    {
        yield return new WaitForSeconds(time);
        additionalMoveX = 0;
        currentState = PlayerState.idle;
    }

    #region Attack functions
    private void attack1()
    {
        attackHandler1.EndAttack();
        
        if (numberOfClicks >= 2)
        {
            animator.SetBool("attack2", true);
            attackHandler2.StartAttack();
        }
        else
        {
            animator.SetBool("attack1", false);
            numberOfClicks = 0;
            currentState = PlayerState.idle;
        }
    }

    private void attack2()
    {
        attackHandler2.EndAttack();
        
        if (numberOfClicks >= 3)
        {
            animator.SetBool("attack3", true);
            attackHandler3.StartAttack();
        }
        else
        {
            animator.SetBool("attack2", false);
            numberOfClicks = 0;
            currentState = PlayerState.idle;
        }
    }

    private void attack3()
    {
        attackHandler3.EndAttack();
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
        numberOfClicks = 0;
        currentState = PlayerState.idle;
    }

    private void jumpAttack1()
    {
        attackHandler1.EndAttack();
        
        if (numberOfClicks >= 2 && !isGrounded)
        {
            animator.SetBool("jumpAttack2", true);
            attackHandler2.StartAttack();
        }
        else
        {
            animator.SetBool("jumpAttack1", false);
            numberOfClicks = 0;
            currentState = PlayerState.idle;
            myRigidbody.gravityScale = 1;
        }
    }

    private void jumpAttack2()
    {
        attackHandler2.EndAttack();
        
        if (numberOfClicks >= 3 && !isGrounded)
        {
            animator.SetBool("jumpAttack3", true);
            attackHandler3.StartAttack();
            myRigidbody.gravityScale = dropSpeed;
            drop = true;
            currentState = PlayerState.drop;
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

    private void jumpaAttack3()
    {
        attackHandler3.EndAttack();
        animator.SetBool("jumpAttack1", false);
        animator.SetBool("jumpAttack2", false);
        animator.SetBool("jumpAttack3", false);
        numberOfClicks = 0;
        currentState = PlayerState.idle;
        myRigidbody.gravityScale = 1;
        drop = false;
    }
    #endregion

    void CreateDust() { dustHandler.CreateDust(); }
}
