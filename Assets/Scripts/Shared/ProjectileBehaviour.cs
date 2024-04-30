using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public float speed = 6f;
    private Vector3 shootDirection;
    private int damage;
    private string casterTag;
    // Update is called once per frame
    void Update()
    {
        transform.position += shootDirection * Time.deltaTime * speed;
    }

    public void Setup(Vector3 shootDirection, int damage, string targetTag)
    {
        this.shootDirection = shootDirection;
        this.damage = damage;
        this.casterTag = targetTag;
        shootDirection = shootDirection.normalized;
        float n = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3 (0, 0, n);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.transform.tag != casterTag)
        {
            var attackable = collider.GetComponent<IAttackable>();
            if (attackable != null)
            {
                attackable.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
