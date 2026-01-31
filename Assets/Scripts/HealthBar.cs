using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;   
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public int maxHealth;
    public void initiliaze()
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
    //    if(Time.time - cur_time >= 1)
      //  {
          //  DecrementHealth(1);
        //    cur_time = Time.time;
        //}

    }

}

