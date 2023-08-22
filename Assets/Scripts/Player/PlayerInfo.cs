using System.Collections;
using UnityEngine;
using Elympics;

public class PlayerInfo : MonoBehaviour, IInitializable
{
    [SerializeField]
    private int playerID;
    [SerializeField]
    private string playerName;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private ElympicsFloat health = new ElympicsFloat();
    [SerializeField]
    private StaggerHandler staggerHandler;
    [SerializeField]
    private PlayerGUI playerGUI;
    
    [SerializeField]
    private float maxShield;
    [SerializeField]
    private ElympicsFloat shield = new ElympicsFloat();
    [SerializeField]
    private float shieldDelay;
    [SerializeField]
    private float shieldReacharge;
    [SerializeField]
    public float armor;

    private float shieldCounter = 0;
    private float shieldRechargeRate = 0.01f;
    private float armorRate = 0.005f;

    public void Initialize()
    {
        health.Value = maxHealth;
        shield.Value = maxShield;
        health.ValueChanged += OnHealthDamaged;
        shield.ValueChanged += OnShieldDamaged;
        playerGUI.InitializeSliders(maxHealth, maxShield);
        playerGUI.InitializeName(playerName);
    }

    private void OnDestroy()
    {
        health.ValueChanged -= OnHealthDamaged;
        shield.ValueChanged -= OnShieldDamaged;
    }

    public void DealDamage(float dmg, bool applyForce)
    {
        dmg -= dmg * armor * armorRate;
        if (shield.Value > dmg)
        {
            shield.Value -= dmg;
            staggerHandler.Stagger(true, applyForce);
        }
        else if (shield.Value < dmg && shield.Value > 0)
        {
            health.Value = health.Value + shield.Value - dmg;
            shield.Value = 0;
            staggerHandler.Stagger(false, applyForce);
        }
        else if(shield.Value == 0)
        {
            health.Value -= dmg;
            staggerHandler.Stagger(false, applyForce);
        }

        if (health.Value <= 0) health.Value = 0;

        shieldCounter = shieldDelay;
    }

    public bool ReplenishHealth(float value)
    {
        if (health.Value == maxHealth)
        {
            return false;
        }
        
        if (value > 0)
        {
            health.Value = Mathf.Min(health.Value + value, maxHealth);
        }

        return true;
    }

    public bool ReplenishShield(float value)
    {
        if (shield.Value == maxShield)
        {
            return false;
        }
        
        if (value > 0)
        {
            shield.Value = Mathf.Min(shield.Value + value, maxShield);
        }
        
        return true;
    }

    public int GetID()
    {
        return playerID;
    }

    public string GetName()
    {
        return playerName;
    }
    
    public void UpdateShield()
    {
        if(gameObject.activeInHierarchy && shieldCounter <= 0 && shield.Value<maxShield)
        {
            shield.Value += shieldReacharge * shieldRechargeRate;
            if (shield.Value > maxShield) shield.Value = maxShield;
        } else if(shieldCounter > 0) shieldCounter -= Time.deltaTime;
    }

    private void Death()
    {
        GetComponent<Animator>().SetBool("death", true);
        GetComponent<PlayerController>().OnDeath();
        StartCoroutine(SafetyDeathClock());
    }

    private IEnumerator SafetyDeathClock()
    {
        yield return new WaitForSeconds(10f);
        GetComponent<PlayerHandler>().OnGameOver();
    }

    private void OnHealthDamaged(float v1, float v2)
    {
        playerGUI.UpdateHealth(v2);

        if (v2 == 0)
        {
            Death();
        }
    }
    
    private void OnShieldDamaged(float v1, float v2)
    {
        playerGUI.UpdateShield(v2);
    }
}
