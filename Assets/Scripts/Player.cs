﻿using System.Collections;
using System.Collections.Generic;
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
    public bool ledgeGrab;
    private bool ledgeJump;
    public Transform groundCheck;
    public Transform ledgeCheck;
    public float checkRadius;
    public LayerMask ground;
    public LayerMask ledge;
    private bool isJumping;
    private bool climbingLedge = false;

    [Header("Attacking")]
    public int numberOfClicks = 0;
    private float lastClickedTime = 0;
    public float maxComboDelay = 1.2f;

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
        if (!climbingLedge) moveInputX = Input.GetAxis("Horizontal");
        if (ledgeGrab) moveInputX = 0;
        if (currentState!=PlayerState.attack && !climbingLedge) { myRigidbody.velocity = new Vector2(moveInputX * speed, myRigidbody.velocity.y); }   //Change during implementing jumping attacks
        else if(currentState==PlayerState.attack) { myRigidbody.velocity = Vector2.zero; }
        if (Input.GetButton("Crouch")) { animator.SetBool("crouch", true); }
        else if (Input.GetButtonUp("Crouch")) { animator.SetBool("crouch", false); }
        if (facingRight == false && moveInputX > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInputX < 0)
        {
            Flip();
        }  
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
        }else
        {
            animator.SetBool("jumping", true);
            isJumping = false;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    private void Update()
    {
        Attack();
        if(currentState != PlayerState.attack) Jump();
        UpdateAnimation();
        Shield();
        Ledge();
    }

    public void Ledge()
    {
        ledgeGrab = Physics2D.OverlapCircle(ledgeCheck.position, checkRadius, ledge);
        if(ledgeJump)
        {
            if (!ledgeGrab) ledgeJump = false;
        }else
        {
            animator.SetBool("ledgeJump", false);
            if (ledgeGrab == true)
            {
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
                    //myRigidbody.velocity = Vector2.up * jumpForce;
                    //if(facingRight) myRigidbody.position = myRigidbody.position + new Vector2(1, 2);
                    //else myRigidbody.position = myRigidbody.position + new Vector2(-1, 2);
                    StartCoroutine(climbeLedge());
                }
                else if(Input.GetButtonDown("Jump") && Input.GetAxis("Vertical") < 0)
                {
                    ledgeJump = true;
                    animator.SetBool("ledgeGrab", false);
                    myRigidbody.gravityScale = 1;
                    animator.SetBool("jumping", false);
                }
            }
            else
            {
                animator.SetBool("ledgeGrab", false);
                myRigidbody.gravityScale = 1;
            }
        }
    }

    IEnumerator climbeLedge()
    {
        float move;
        if (facingRight) move = 5f;
        else move = -5f;
        climbingLedge = true;
        myRigidbody.velocity = Vector2.up * jumpForce * 1.5f;
        yield return new WaitForSeconds(0.3f);
        myRigidbody.velocity = Vector2.right * move;
        yield return new WaitForSeconds(0.2f);
        climbingLedge = false;
    }

    private void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
        if (isGrounded == true)
        {
            extraJumps = extraJumpsMax;
            animator.SetBool("isGrounded", true);
        } else
        {
            animator.SetBool("isGrounded", false);
        }
        if (Input.GetButtonDown("Jump") && extraJumps > 0)
        {
            myRigidbody.velocity = Vector2.up * jumpForce;
            extraJumps--;
            isJumping = true;
        }
        else if (Input.GetButtonDown("Jump") && extraJumps == 0 && isGrounded == true)
        {
            myRigidbody.velocity = Vector2.up * jumpForce;
            isJumping = true;
        }
    }

    public void Attack()
    {
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numberOfClicks = 0;
        }
        if (Input.GetButtonDown("attack") && isGrounded && !isJumping)
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
    }

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
}
