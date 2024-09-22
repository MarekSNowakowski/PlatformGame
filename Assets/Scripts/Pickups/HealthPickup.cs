using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealthPickup : Pickup
{
    [SerializeField]
    private float value;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (col.GetComponent<PlayerInfo>().ReplenishHealth(value))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
