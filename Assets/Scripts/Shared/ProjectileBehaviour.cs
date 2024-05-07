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
        transform.position += shootDirection * Time.deltaTime * speed;
        if (Vector3.Distance(transform.position, startPosition) >= distance)
        {
            Destroy(gameObject);
        }
    }

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
            else if (collider.GetComponent<StockpileScript>() == null && collider.gameObject.layer != LayerMask.NameToLayer("Items"))
            {
                Destroy(gameObject);
            }
        }
    }
}
