using UnityEngine;

public class GlassScript : Destructible
{

    [SerializeField]private GameObject ToxicGasPrefab;


    private void Awake()
    {
        maxHealth = 1f;
        currentHealth = maxHealth;
    }


    private void glassBreaked()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.GlassBreak);
        GameObject ToxicGasPrefab = Instantiate(this.ToxicGasPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected override void Die()
    {
        glassBreaked();
    }

}
