using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private int maxHealth;
    public int cur_health;
    private void Awake()
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }
    
    public void DecrementHealth(int dmg)
    {
        slider.value -= dmg;
    }

    public void IncrementHealth(int health)
    {
        slider.value += health;
    }
}
