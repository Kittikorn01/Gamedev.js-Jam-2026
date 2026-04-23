using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3; 
    public float currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        //Debug.Log(gameObject.name + " health: " + currentHealth); // Current health 

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject); // if health is 0 destroy enemy
        }
    }

    public void Heal(float heal)
    {
        currentHealth += heal;
        healthBar.SetHealth(currentHealth, maxHealth);
    }
}
