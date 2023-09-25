using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public int MaxHealth = 100;
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
    }  
}
