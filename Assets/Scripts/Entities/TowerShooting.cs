using UnityEngine;

public class TowerShooting : MonoBehaviour
{
    public Transform bulletpoint;
    public GameObject bulletPrefab;
    public float cooldownTime = 2.0f; // Time Cooldown 
    private float nextFireTime = 2.0f;  // Next time to shoot

    void Update()
    {
        // Shoot every x seconds
        if (Time.time > nextFireTime)
        {
            Debug.Log("Shooting!");
            //Debug.Log(Time.time);
            nextFireTime = Time.time + cooldownTime; // Next time to shoot is current time + cooldown
            shoot();
        }
    }

    void shoot()
    {
        Instantiate(bulletPrefab, bulletpoint.position, bulletpoint.rotation);
    }
}
