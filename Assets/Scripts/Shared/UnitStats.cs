using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int attackDamage = 0;
    public float attackSpeed = 2f;
    public float attackCooldown = 0f;
    public int gatheringDamage = 10;
    public float gatheringSpeed = 4f;
    public float gatherCooldown = 0f;
    int currentHealth;

    public HealthBarScript healthBar;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

    }

    public void TakeDamage(int Damage)
    {
        currentHealth -= Damage;

        healthBar.SetHealth(currentHealth);
        // Play hurt animation

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void Die() 
    {
        //die animation

        //disable enemy
        Destroy(gameObject);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
