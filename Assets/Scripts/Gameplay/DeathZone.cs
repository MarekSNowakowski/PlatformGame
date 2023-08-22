using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeathZone : MonoBehaviour
{
    [SerializeField] private float startRadius = 100;
    [SerializeField] private float endRadius = 5;
    [SerializeField] private float shrinkTime = 60;
    [SerializeField] private float damageValue = 3;
    [SerializeField] private float damageFrequency = 1;
    
    private float shrinkTimer = 0;
    private float damageTimer = 0;

    private Dictionary<int, PlayerInfo> playersInDamageZone = new();

    private void Start()
    {
        transform.localScale = Vector3.one * startRadius;
        StartShrinking();
    }

    private void Update()
    {
        if (playersInDamageZone.Count > 0)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer > damageFrequency)
            {
                damageTimer = 0;
                foreach (KeyValuePair<int, PlayerInfo> player in playersInDamageZone)
                {
                    player.Value.DealDamage(damageValue, false);
                }
            }
        }
        else
        {
            damageTimer = 0;
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

    public void StartShrinking()
    {
        StartCoroutine(ZoneShrinking());
    }

    private IEnumerator ZoneShrinking()
    {
        while (shrinkTimer < shrinkTime)
        {
            shrinkTimer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one * startRadius,  Vector3.one * endRadius, shrinkTimer / shrinkTime);
            yield return null;
        }
        
        transform.localScale = Vector3.one * endRadius;
    }
}
