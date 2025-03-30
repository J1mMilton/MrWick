using UnityEngine;

public enum ItemType
{
    HealthPack,
    Invulnerability,
    AttackDamageBoost
}

public class ItemPickup : MonoBehaviour
{
    public ItemType itemType;
    
    // Parameters for different item types.
    public int healthAmount = 20;
    public float invulnerabilityDuration = 3f;
    public float attackDamageBoostAmount = 10f;
    public float attackDamageBoostDuration = 5f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (itemType)
                {
                    case ItemType.HealthPack:
                        player.RecoverHealth(healthAmount);
                        break;
                    case ItemType.Invulnerability:
                        player.AddInvulnerability(invulnerabilityDuration);
                        break;
                    case ItemType.AttackDamageBoost:
                        player.AddAttackDamageBoost(attackDamageBoostAmount, attackDamageBoostDuration);
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}