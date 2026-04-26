using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 5f;
    private bool isMoving = true;

    public float attackCooldown = 2f;
    private float attackTimer = 0f;
    public float damage = 1.0f;

    private Health targetTower;

    public Sprite[] enemyFaces; // กล่องเก็บรูปมอนสเตอร์หลายๆ แบบ
    private SpriteRenderer mySpriteRenderer;

    void Awake()
    {
        // ให้โค้ดวิ่งหา SpriteRenderer ในตัวมันเองรอไว้เลย
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        speed = Random.Range(1f, 1.8f);

        if (enemyFaces.Length > 0 && mySpriteRenderer != null)
        {
            // สุ่มหยิบรูปมา 1 รูป
            int randomFaceIndex = Random.Range(0, enemyFaces.Length);
            // เปลี่ยนหน้าตาให้ตัวมันเอง!
            mySpriteRenderer.sprite = enemyFaces[randomFaceIndex];
        }
    }

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

        float wobble = Mathf.Sin(Time.time * 10f) * 5f;
        transform.rotation = Quaternion.Euler(0, 0, wobble);
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
