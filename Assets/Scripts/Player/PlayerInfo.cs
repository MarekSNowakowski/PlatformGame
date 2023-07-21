using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elympics;

public class PlayerInfo : MonoBehaviour, IObservable, IInitializable
{
    [SerializeField]
    private int playerID;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private ElympicsFloat health = new ElympicsFloat();
    [SerializeField]
    private PlayerHandler playerHandler;
    [SerializeField]
    private StaggerHandler staggerHandler;

    private void OnDestroy()
    {
        health.ValueChanged -= OnDamaged;
    }

    public void DealDamage(float damage)
    {
        health.Value -= damage;
    }

    public void Initialize()
    {
        health.Value = maxHealth;
        health.ValueChanged += OnDamaged;
    }

    public int GetID()
    {
        return playerID;
    }

    private void OnDamaged(float v1, float v2)
    {
        if (v1 > v2)
        {
            staggerHandler.Stagger();
        }
    }
}
