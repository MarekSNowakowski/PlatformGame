using System;
using UnityEngine;

public class GameplaySoundsManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Header("Audio Sources")]
    [SerializeField] private ManagedAudioSource jumpSound;
    [SerializeField] private ManagedAudioSource walkSound;
    [SerializeField] private ManagedAudioSource landSound;
    [SerializeField] private ManagedAudioSource grabSound;
    [SerializeField] private ManagedAudioSource slideSound;

    [SerializeField] private ManagedAudioSource attackSound1;
    [SerializeField] private ManagedAudioSource attackSound2;
    [SerializeField] private ManagedAudioSource attackSound3;
    
    [SerializeField] private ManagedAudioSource jumpAttackSound1;
    [SerializeField] private ManagedAudioSource jumpAttackSound2;
    [SerializeField] private ManagedAudioSource jumpAttackSound3;
    
    [SerializeField] private ManagedAudioSource healSound;
    [SerializeField] private ManagedAudioSource shieldSound;
    
    [SerializeField] private ManagedAudioSource healhDamagedSound;
    [SerializeField] private ManagedAudioSource shieldDamagedSound;
    [SerializeField] private ManagedAudioSource deathSound;

    private bool isGrounded = true;
    private bool ledgeGrab = true;
    
    private const string groundedBoolName = "isGrounded";
    private const string grabBoolName = "ledgeGrab";
    
    private void Update()
    {
        ManageLandingSound();
        ManageGrabSound();
    }

    private void ManageLandingSound()
    {
        if (!isGrounded)
        {
            if (animator.GetBool(groundedBoolName))
            {
                isGrounded = true;
                landSound.AudioSource.Play();
            }
        }
        else
        {
            if (!animator.GetBool(groundedBoolName))
            {
                isGrounded = false;
            }
        }
    }
    
    private void ManageGrabSound()
    {
        if (!ledgeGrab)
        {
            if (animator.GetBool(grabBoolName))
            {
                ledgeGrab = true;
                grabSound.AudioSource.Play();
            }
        }
        else
        {
            if (!animator.GetBool(grabBoolName))
            {
                ledgeGrab = false;
            }
        }
    }

    public void PlayJumpSound()
    {
        jumpSound.AudioSource.Play();
    }
    
    public void PlayWalkSound()
    {
        walkSound.AudioSource.Play();
    }

    public void PlaySlideSound()
    {
        slideSound.AudioSource.Play();
    }

    public void PlayHealSound()
    {
        healSound.AudioSource.Play();
    }

    public void PlayShieldSound()
    {
        shieldSound.AudioSource.Play();
    }

    public void PlayAttack1Sound()
    {
        attackSound1.AudioSource.Play();
    }
    
    public void PlayAttack2Sound()
    {
        attackSound2.AudioSource.Play();
    }
    
    public void PlayAttack3Sound()
    {
        attackSound3.AudioSource.Play();
    }
    
    public void PlayJumpAttack1Sound()
    {
        jumpAttackSound1.AudioSource.Play();
    }
    
    public void PlayJumpAttack2Sound()
    {
        jumpAttackSound2.AudioSource.Play();
    }
    
    public void PlayJumpAttack3Sound()
    {
        jumpAttackSound3.AudioSource.Play();
    }

    public void PlayHealthDamagedSound()
    {
        healhDamagedSound.AudioSource.Play();
    }
    
    public void PlayShieldDamagedSound()
    {
        shieldDamagedSound.AudioSource.Play();
    }

    public void PlayDeathSound()
    {
        deathSound.AudioSource.Play();
    }
}
