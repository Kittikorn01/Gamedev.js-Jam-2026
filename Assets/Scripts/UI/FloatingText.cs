using UnityEngine;
using UnityEngine.UI; // ถ้าใช้ TextMeshPro ให้เปลี่ยนเป็น using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 50f; // ความเร็วในการลอยขึ้น
    public float destroyTime = 1f; // ลอยอยู่ 1 วินาทีแล้วค่อยหายไป

    void Start()
    {
        // สั่งตั้งเวลาทำลายตัวเองล่วงหน้าเลย
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // สั่งให้ตัวหนังสือค่อยๆ ลอยขึ้นไปข้างบน
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }
}