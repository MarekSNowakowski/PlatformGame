using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    [SerializeField]
    private PlayerInfo playerInfo;
    [SerializeField]
    private float hitDamage;

    private bool alreadyHit;
    private long attackEndTick;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyHit) return;
        if (other.TryGetComponent(out PlayerInfo hitPlayer))
        {
            if (playerInfo.GetID() != hitPlayer.GetID())
            {
                hitPlayer.DealDamage(hitDamage, true);
                alreadyHit = true;
            }
        }
    }

    public void StartAttack()
    {
        gameObject.SetActive(true);
        alreadyHit = false;
    }

    public void EndAttack()
    {
        gameObject.SetActive(false);
    }
}
