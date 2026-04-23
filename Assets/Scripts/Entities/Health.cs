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
            Die(); // if health is 0 destroy enemy
        }
    }

    void Die()
    {
        // 1. เช็คก่อนว่าคนที่ตายเนี่ย ใช่ "Enemy" ไหม? 
        // (เผื่อป้อมตาย จะได้ไม่ไปกวนระบบนับแต้มศัตรู)
        if (gameObject.CompareTag("Enemy"))
        {
            // ตะโกนบอก WaveManager ว่าหนูตายแล้วจ้า!
            if (WaveManager.instance != null)
            {
                WaveManager.instance.EnemyKilled();
            }
        }

        // 2. ทำลาย Object ตัวเองทิ้ง (โค้ดเดิมของตั้ม)
        Destroy(gameObject);
    }

    public void Heal(float heal)
    {
        currentHealth += heal;
        healthBar.SetHealth(currentHealth, maxHealth);
    }
}
