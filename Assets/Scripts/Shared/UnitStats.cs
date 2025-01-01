/**
* File: UnitStats.cs
* Author: Kryštof Glos
* Date Last Modified: 1.5.2024
* Description: This script defines the stats and behavior of units in the game.
*/
using UnityEngine;

public class UnitStats : MonoBehaviour, IAttackable
{
    public string characterName;
    public int maxHealth = 100;
    public int healthRegeneration = 5;
    public int attackDamage = 0;
    public float attackSpeed = 2f;
    [HideInInspector]
    public float attackCooldown = 0f;
    public int gatheringDamage = 10;
    public float gatheringSpeed = 4f;
    [HideInInspector]
    public float gatherCooldown = 0f;
    public float attackRange = 2f;
    public float movementSpeed = 1.5f;

    private int currentHealth;
    private float timer = 0;
    private float regenTimer = 0;
    private float regenerationTimer = 5;

    public HealthBarScript healthBar;
    public ProjectileBehaviour projectilePrefab;
    public bool isRanged;

    public int totalLevel = 1;
    public int treeCuttingLevel = 1;
    public int miningLevel = 1;
    public int gatheringLevel = 1;
    public int fightingLevel = 1;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

    }

    void Update()
    {
        if (currentHealth != maxHealth)
        {
            timer += Time.deltaTime;
            if (timer >= regenerationTimer)
            {
                regenTimer += Time.deltaTime;
                if (regenTimer >= 1)
                {
                    regenTimer = 0;
                    RegenerateHealth();
                }
            }
        }
    }

    public void TakeDamage(int Damage)
    {
        timer = 0;
        currentHealth -= Damage;

        healthBar.SetHealth(currentHealth);
        // Play hurt animation

        if (currentHealth <= 0)
        {
            Die();
            if (tag == "selectable")
                EventManager.GetInstance().playerUnitCount--;
        }
    }

    // Regenerate health over time
    public void RegenerateHealth()
    {
        if (currentHealth != maxHealth && currentHealth + healthRegeneration <= maxHealth)
        {
            currentHealth += healthRegeneration;
            healthBar.SetHealth(currentHealth);
        }
    }

    // Handle death of the unit
    void Die()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Destroy(gameObject);
    }
}
