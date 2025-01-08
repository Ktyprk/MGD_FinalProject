using UnityEngine;

public class TargetHealthController : MonoBehaviour
{
    public int health = 1000;
    
    
    
    public void TargetTakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Destroy(gameObject);
        }
        Debug.Log("Taken " + damage + " damage. Remaining Health: " + health);
    }
}
