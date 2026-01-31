using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private int health = 100;
    [SerializeField] private GameObject healthBarObj;
    private HealthBar healthBar;    
    private void Awake()
    {
        healthBar = healthBarObj.GetComponent<HealthBar>();
        healthBar.maxHealth = health;
        healthBar.initiliaze();
    }
    public void TakeDamage(int damage)
    {
        healthBar.DecrementHealth(damage);
    }   
}
