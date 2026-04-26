using UnityEngine;
using UnityEngine.SceneManagement; // ต้องใช้ตัวนี้เพื่อทำปุ่ม Retry โหลดฉากใหม่

public class BaseHealth : MonoBehaviour
{
    [Header("Base Settings")]
    public int maxHealth = 3; // โควต้าพลาดได้ 3 ตัว
    private int currentHealth;

    [Header("UI Elements")]
    public GameObject losePanel; // เอาไว้ใส่หน้าต่าง Game Over ที่เราสร้างไว้

    [Header("UI System")]
    public GameObject[] heartsUI; // อาเรย์สำหรับเก็บรูปหัวใจ

    [Header("SFX Sounds")]
    public AudioSource audioSource;
    public AudioClip enemyLeakSound;
    void Start()
    {
        currentHealth = maxHealth;

        // 1. ซ่อนหน้าต่างแพ้ตอนเริ่มเกม (เผื่อลืมซ่อนในหน้า Scene)
        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        // 2. ทำให้แน่ใจว่าเวลาเดินปกติ (เผื่อกด Retry มาจากรอบที่แล้วที่เวลาโดนหยุดไว้)
        Time.timeScale = 1f;
    }

    // 3. ฟังก์ชันนี้จะทำงานอัตโนมัติเมื่อมีคนเดินมาชนกล่องเรดาร์ (Trigger) ของเรา
    private void OnTriggerEnter2D(Collider2D other)
    {
        // เช็คก่อนว่าคนที่มาชน ใช่ "Enemy" หรือเปล่า?
        if (other.CompareTag("Enemy"))
        {
            currentHealth--; // เลือดบ้านลดลง 1
            Debug.Log($"<color=orange>ศัตรูหลุดเข้าบ้าน!</color> เลือดบ้านเหลือ: {currentHealth}/{maxHealth}");

            UpdateHeartsUI();

            // +++ เพิ่ม 3 บรรทัดนี้เข้าไป เพื่อบอก WaveManager ว่ามันหลุดเข้ามานะ! +++
            if (WaveManager.instance != null)
            {
                WaveManager.instance.EnemyEscaped();
            }

            // ลบศัตรูตัวนั้นทิ้งไปเลย จะได้ไม่เดินทะลุจอไปเรื่อยๆ จนกินสเปคเครื่อง
            Destroy(other.gameObject);

            // 4. เช็คเงื่อนไขแพ้
            if (currentHealth <= 0)
            {
                TriggerLoseCondition();
            }
        }
    }

    private void UpdateHeartsUI()
    {
        // วนลูปเช็คว่าเลือดเหลือเท่าไหร่ ก็เปิดหัวใจแค่นั้น
        for (int i = 0; i < heartsUI.Length; i++)
        {
            if (i < currentHealth)
            {
                heartsUI[i].SetActive(true); // เลือดถึง เปิดไว้
            }
            else
            {
                heartsUI[i].SetActive(false); // เลือดไม่ถึง ปิดทิ้งไป
            }
        }
    }

    void TriggerLoseCondition()
    {
        Debug.Log("<color=red>บ้านแตก! GAME OVER!</color>");

        // เปิดหน้าต่างแพ้โชว์ขึ้นมา
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        // 5. หยุดเวลาในเกม! (ท่าไม้ตาย: มอนสเตอร์จะหยุดเดิน ป้อมจะหยุดยิง อนิเมชั่นจะค้าง)
        Time.timeScale = 0f;
    }

    // ----------------------------------------------------
    // แถมให้! ฟังก์ชันเอาไว้ผูกกับปุ่ม "Retry" ในหน้า Game Over
    // ----------------------------------------------------
    public void RetryGame()
    {
        // ปลดล็อคเวลาให้กลับมาเดินปกติก่อน (ไม่งั้นโหลดฉากใหม่เกมก็ค้างอยู่ดี)
        Time.timeScale = 1f;

        // สั่งโหลด Scene ปัจจุบันใหม่ทั้งหมด (เริ่มเกมใหม่)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}