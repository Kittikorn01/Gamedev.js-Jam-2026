using UnityEngine;

public class TowerShooting : MonoBehaviour
{
    public Transform bulletpoint;
    public GameObject bulletPrefab;
    public float cooldownTime = 2.0f; // Time Cooldown 
    private float nextFireTime = 0;  // Next time to shoot

    void Start()
    {
        nextFireTime = Time.time + cooldownTime; // Start shooting after cooldown time
    }
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
