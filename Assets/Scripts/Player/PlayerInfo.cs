using UnityEngine;
using Elympics;

public class PlayerInfo : MonoBehaviour, IObservable, IInitializable
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

    public void DealDamage(float dmg)
    {
        dmg -= dmg * armor * armorRate;
        if (shield.Value > dmg) shield.Value -= dmg;
        else if (shield.Value < dmg && shield.Value > 0)
        {
            health.Value = health.Value + shield.Value - dmg;
            shield.Value = 0;
        }
        else if(shield.Value == 0)
        {
            health.Value -= dmg;
        }
        if (health.Value <= 0) Death();

        shieldCounter = shieldDelay;
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

    public void Death()
    {
        gameObject.SetActive(false);
    }

    private void OnHealthDamaged(float v1, float v2)
    {
        if (v1 > v2 && Mathf.Abs(v2-v1) > 1)
        {
            staggerHandler.Stagger(false);
        }
        playerGUI.UpdateHealth(v2);
    }
    
    private void OnShieldDamaged(float v1, float v2)
    {
        if (v1 > v2 && Mathf.Abs(v2-v1) > 1)
        {
            staggerHandler.Stagger(true);
        }
        playerGUI.UpdateShield(v2);
    }
}
