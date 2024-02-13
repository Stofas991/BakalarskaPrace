using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public int MaxHealth = 100;
    public int attackDamage = 0;
    public float attackSpeed = 2f;
    public float attackCooldown = 0f;
    public int plantCutDamage = 10;
    public float plantCutSpeed = 4f;
    public float cutCooldown = 0f;
    int CurrentHealth;

    public HealthBarScript HealthBar;
    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        HealthBar.SetMaxHealt(MaxHealth);

    }

    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;

        HealthBar.SetHealth(CurrentHealth);
        // Play hurt animation

        if (CurrentHealth <= 0)
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
