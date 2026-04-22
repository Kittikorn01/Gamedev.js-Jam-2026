using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 5f;
    private bool isMoving = true;

    public float attackCooldown = 2f;
    private float attackTimer = 0f;
    public int damage = 1;

    private Health targetTower;

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }else if (targetTower != null){
            // Enemy hits the tower and starts attacking
            enemyAttack();
        }
        else
        {
            // Tower is destroyed, enemy can move again
            isMoving = true;
        }
    }

    void enemyAttack()
    {
        // Enemy stop moving and start attacking the tower
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            targetTower.TakeDamage(damage); 
            Debug.Log("Enemy Attack tower");
            attackTimer = 0f; // รีเซ็ตเวลาเพื่อเริ่มง้างตีรอบใหม่
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            Debug.Log("Enemy hit tower");
            isMoving = false;
            targetTower = collision.GetComponent<Health>();
            attackTimer = attackCooldown; // Enemy hit the tower start attacking
        }
    }
}
