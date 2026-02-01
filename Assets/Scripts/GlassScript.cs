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

        //Toxic gas is leaked when glass is broken
        GameObject ToxicGasPrefab = Instantiate(this.ToxicGasPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject); //destroy glass object
    }

    protected override void Die()
    {
        glassBreaked();
    }

}
