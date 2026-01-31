using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;   
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private int maxHealth;
    public int cur_health;
    private float cur_time = 0f;
    private Coroutine healthCoroutine;
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

    public void Update()
    {
        if(Time.time - cur_time >= 1)
        {
            DecrementHealth(1);
            cur_time = Time.time;
        }

    }

}

