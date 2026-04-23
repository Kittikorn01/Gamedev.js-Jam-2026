using System.Collections.Generic;
using UnityEngine;

public class TowerShooting : MonoBehaviour
{
    public Transform bulletpoint;
    public GameObject bulletPrefab;
    public float cooldownTime = 2.0f; // Time Cooldown 
    private float nextFireTime = 0;  // Next time to shoot

    private List<GameObject> enemiesInRange = new List<GameObject>();

    void Start()
    {
        nextFireTime = Time.time + cooldownTime; // Start shooting after cooldown time
    }
    void Update()
    {
        // 2. เคลียร์ศพ: ถ้าศัตรูตัวไหนตาย (หายไปจากฉาก) ให้ลบออกจากเรดาร์ทันที
        enemiesInRange.RemoveAll(enemy => enemy == null);

        // 3. เงื่อนไขการยิง: "ต้องมีศัตรูในระยะ" และ "ดีเลย์ปืนพร้อมยิงแล้ว"
        if (enemiesInRange.Count > 0 && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + cooldownTime;
            shoot();
        }
    }

    void shoot()
    {
        Instantiate(bulletPrefab, bulletpoint.position, bulletpoint.rotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject); // เจอศัตรู! จดลงบัญชีดำ
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject); // ศัตรูเดินหนีไปแล้ว ขีดฆ่าออกจากบัญชี
        }
    }
}
