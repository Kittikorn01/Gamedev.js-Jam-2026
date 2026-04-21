using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3; 
    [SerializeField] private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " health: " + currentHealth); // Current health 

        if (currentHealth <= 0)
        {
            Destroy(gameObject); // if health is 0 destroy enemy
        }
    }
}
