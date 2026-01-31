using UnityEngine;

public class GodClicker : MonoBehaviour
{
    void Update()
    {
        // On Left Click
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from mouse position into the world
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                // Try to find an IDamageable thing
                IDamageable target = hit.collider.GetComponent<IDamageable>();
                if (target != null)
                {
                    Debug.Log($"Smited: {hit.collider.name}");
                    target.TakeDamage(50f); // Deal 50 damage
                }
            }
        }
    }
}