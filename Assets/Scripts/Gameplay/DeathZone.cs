using System;
using System.Collections;
using System.Collections.Generic;
using Elympics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeathZone : ElympicsMonoBehaviour, IUpdatable, IInitializable, IObservable
{
    [SerializeField] private float startRadius = 100;
    [SerializeField] private float endRadius = 5;
    [SerializeField] private float shrinkTime = 80;
    [SerializeField] private float damageValue = 3;
    [SerializeField] private float damageFrequency = 1;
    [SerializeField] private GameplayTimer gameplayTimer;
    [SerializeField] private int shrinkStartTime = 120;
    
    private readonly ElympicsBool shrinkingStarted = new ElympicsBool(false);
    private readonly ElympicsFloat shrinkTimer = new ElympicsFloat(0);
    private readonly ElympicsFloat damageTimer = new ElympicsFloat(0);

    private Dictionary<int, PlayerInfo> playersInDamageZone = new();

    
    public void Initialize()
    {
        transform.localScale = Vector3.one * startRadius;
    }
    
    public void ElympicsUpdate()
    {
        if (!shrinkingStarted.Value && gameplayTimer.GetTimeLeft() <= shrinkStartTime)
        {
            shrinkingStarted.Value = true;
            gameplayTimer.OnSafeZoneShrinkingStart();
        }
        
        if (playersInDamageZone.Count > 0)
        {
            damageTimer.Value += Elympics.TickDuration;
            if (damageTimer > damageFrequency)
            {
                damageTimer.Value = 0;
                foreach (KeyValuePair<int, PlayerInfo> player in playersInDamageZone)
                {
                    player.Value.DealDamage(damageValue, false);
                }
            }
        }
        else
        {
            damageTimer.Value = 0;
        }

        if (shrinkingStarted)
        {
            if (shrinkTimer < shrinkTime)
            {
                shrinkTimer.Value += Elympics.TickDuration;
                transform.localScale = Vector3.Lerp(Vector3.one * startRadius,  Vector3.one * endRadius, shrinkTimer / shrinkTime);
            }
            else
            {
                transform.localScale = Vector3.one * endRadius;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerInfo playerInfo) && !other.isTrigger)
        {
            playersInDamageZone.Remove(playerInfo.GetID());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerInfo playerInfo))
        {
            playersInDamageZone.TryAdd(playerInfo.GetID(), playerInfo);
        }
    }
}
