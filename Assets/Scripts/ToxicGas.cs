using UnityEngine;

public class ToxicGas : MonoBehaviour
{
    private CircleCollider2D toxicGasCollider;
    private int damage = 5;
    private int max_radius = 3;
    private float expansionRate = 0.5f;
    private float startTime = 0f;
    private float endTime = 7f;
    private void Awake()
    {
        toxicGasCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime > endTime)
            Destroy(gameObject);

        if (toxicGasCollider.radius < max_radius)
        {
            toxicGasCollider.radius += expansionRate * Time.deltaTime;

        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //PlayerHealth component deals with player health management
        //I've created PlayerHealth script but its not attached to Player prefab yet
        if (collision.GetComponent<Player>())
        {
            
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            player.TakeDamage(damage);
        }
    }
    private void OnDrawGizmos()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, col.radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, max_radius);
    }


}
