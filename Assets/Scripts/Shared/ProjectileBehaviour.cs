/**
* File: ProjectileBehaviour.cs
* Author: Kryštof Glos
* Date Last Modified: 3.5.2024
* Description: This script defines the behavior of projectiles in the game.
*/
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed = 6f;
    private Vector3 shootDirection;
    private int damage;
    private string casterTag;
    private Vector3 startPosition;
    private int distance = 20;
    // Update is called once per frame
    void Update()
    {
        // Move the projectile along its shoot direction with a specified speed
        transform.position += shootDirection * Time.deltaTime * speed;

        // Destroy the projectile if it travels beyond a certain distance from its start position
        if (Vector3.Distance(transform.position, startPosition) >= distance)
        {
            Destroy(gameObject);
        }
    }

    // Set up the projectile with initial parameters
    public void Setup(Vector3 shootDirection, int damage, string targetTag)
    {
        startPosition = transform.position;
        this.shootDirection = shootDirection;
        this.damage = damage;
        this.casterTag = targetTag;
        shootDirection = shootDirection.normalized;
        float n = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, n);
    }

    // Handle collision with other objects
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the collider is not the same tag as the caster
        if (collider.transform.tag != casterTag)
        {
            // Attempt to damage the collider if it implements the IAttackable interface
            var attackable = collider.GetComponent<IAttackable>();
            if (attackable != null)
            {
                attackable.TakeDamage(damage);
                Destroy(gameObject);
            }
            // Destroy the projectile if it collides with something other than an attackable object or a stockpile
            else if (collider.GetComponent<StockpileScript>() == null && collider.gameObject.layer != LayerMask.NameToLayer("Items"))
            {
                Destroy(gameObject);
            }
        }
    }
}
