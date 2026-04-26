using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class RepairMinigame : MonoBehaviour
{
    [Header("UI References")]
    public Slider repairSlider;
    public RectTransform greenZone; 
    public GameObject repairGroup;

    [Header("Settings")]
    public float minSpeed = 1.0f;
    public float maxSpeed = 3.0f;

    [Header("Zone Settings")]
    public float zoneWidth = 0.15f; // width of green zone

    [Header("Tower Reference")]
    // เปลี่ยนคำว่า TowerHealth เป็นชื่อสคริปต์เลือดป้อมของตั้มจริงๆ นะ
    public Health myTowerHealth;

    [Header("UI Visuals")]
    public GameObject repairVisuals; // ลากตัว RepairVisuals มาใส่ช่องนี้

    [Header("UI Feedback")]
    public GameObject floatingTextPrefab; // ใส่ Prefab ตัวอักษรลอย
    public Transform textSpawnPoint; // ใส่ตำแหน่งที่จะให้ตัวเลขเด้ง (ลาก Canvas หรือ RepairVisuals มาใส่ก็ได้)
    public GameObject lockoutPanel; // ใส่ป้ายสีแดง Overheat

    [Header("SFX Sounds")]
    public AudioSource audioSource;
    public AudioClip successSound; // เสียงซ่อมผ่าน (+3)
    public AudioClip failSound;    // เสียงซ่อมพลาด (-1)

    private bool isRepairActive = false; // ตัวแปรจำสถานะว่าหน้าต่างเปิดอยู่ไหม

    private float currentSpeed;
    private float progress = 0f; // record the current position of the needle (0 to 1)
    private int direction = 1;   // 1 for moving right, -1 for moving left
    private int missCount = 0;
    private bool isLockout = false;
    private float targetMin;
    private float targetMax;
    //private bool movingForward = true; // ใช้เช็คทิศทางเข็ม

    public float lockoutTimer = 0f; // ตัวแปรจับเวลาการแบน

    void Start()
    {
        // ท่าไม้ตาย: สั่งให้ UI ซ่อม วิ่งไปหา Component Health บนตัวพ่อ (ป้อม) อัตโนมัติ
        if (myTowerHealth == null)
        {
            myTowerHealth = GetComponentInParent<Health>();

            if (myTowerHealth == null)
            {
                Debug.LogError("หาหลอดเลือดป้อมไม่เจอ! แน่ใจนะว่า UI นี้อยู่ในป้อม?");
            }
        }

        RandomizeSpeed();
        RandomizeGreenZone();
    }

    void Update()
    {
        // 1. ระบบเช็คเลือดเปิด/ปิดมินิเกม
        if (myTowerHealth != null)
        {
            float currentHP = myTowerHealth.currentHealth; // เปลี่ยนให้ตรงกับชื่อตัวแปรใน Health.cs
            float maxHP = myTowerHealth.maxHealth;

            if (maxHP > 0)
            {
                // เงื่อนไขเปิด: เลือดต่ำกว่าหรือเท่ากับ 90% และยังไม่ได้เปิด
                if (currentHP <= maxHP * 0.9f && !isRepairActive)
                {
                    isRepairActive = true;
                    repairVisuals.SetActive(true);
                }
                // เงื่อนไขปิด: เลือดเต็ม 100% และกำลังเปิดอยู่
                else if (currentHP >= maxHP && isRepairActive)
                {
                    isRepairActive = false;
                    repairVisuals.SetActive(false);
                    missCount = 0;
                }
            }
        }

        if (!isRepairActive || isLockout) return;

        // 1. update the needle position based on current speed and direction   
        progress += currentSpeed * Time.deltaTime * direction;

        // 2. update the slider value to reflect the current position of the needle
        repairSlider.value = progress;

        // 3. check if the needle has reached the end of the slider and reverse direction if needed
        if (progress >= 1f)
        {
            progress = 1f;      // lock is not allowed to go beyond the slider
            direction = -1;     // move backward
            RandomizeSpeed();   // randomize speed again
        }
        else if (progress <= 0f)
        {
            progress = 0f;      
            direction = 1;      
            RandomizeSpeed();   
        }

        

    }

    void RandomizeSpeed()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void RandomizeGreenZone()
    {
        // 1. สุ่มค่าขอบซ้ายสุด (สุ่มตั้งแต่ 0 ไปจนถึงจุดที่เผื่อความกว้างไว้แล้ว จะได้ไม่ทะลุขวา)
        targetMin = Random.Range(0f, 1f - zoneWidth);

        // 2. คำนวณขอบขวาสุด
        targetMax = targetMin + zoneWidth;
        Debug.Log($"<color=green>สุ่มโซนเขียวใหม่:</color> ซ้ายสุดที่ {targetMin:F2} | ขวาสุดที่ {targetMax:F2}");
        // 3. สั่งย้ายตำแหน่งภาพโซนสีเขียวใน UI (ใช้ท่าไม้ตาย Anchors)
        if (greenZone != null)
        {
            // ตั้งจุดยึดซ้าย (Min) และขวา (Max) ตามค่าที่เราสุ่มได้
            greenZone.anchorMin = new Vector2(targetMin, 0);
            greenZone.anchorMax = new Vector2(targetMax, 1);

            // รีเซ็ตขอบภาพให้พอดีกับจุดยึดเป๊ะๆ
            greenZone.offsetMin = Vector2.zero; // ลบขยะค่าพิกเซลเดิมทิ้ง
            greenZone.offsetMax = Vector2.zero;
        }
    }

    // ฟังก์ชันนี้ไว้ลากไปใส่ในช่อง OnClick() ของ Button ใน Unity
    public void OnRepairClick()
    {
        Debug.Log("<color=yellow>ใครแอบกดปุ่มมมม!?</color>");
        if (isLockout) return; // ถ้าโดนแบนอยู่ กดไปก็ไม่มีอะไรเกิดขึ้น

        // เช็คว่าตำแหน่งเข็ม (progress) อยู่ระหว่างโซนเขียวไหม
        if (progress >= targetMin && progress <= targetMax)
        {
            HandleSuccess();
        }
        else
        {
            HandleFail();
        }
    }

    void HandleSuccess()
    {
        // +++ ใส่บรรทัดนี้ลงไป มันจะฟ้องหมดเปลือกว่าใครเรียกมันมา! +++
        Debug.Log("<color=orange>ผู้ต้องสงสัยที่แอบเรียก HandleSuccess คือ: \n</color>" + StackTraceUtility.ExtractStackTrace());
        Debug.Log("<color=cyan>ซ่อมสำเร็จ!</color>");
        //missCount = 0; // รีเซ็ตแต้มพลาด
        audioSource.PlayOneShot(successSound);
        if (myTowerHealth != null)
        {
            myTowerHealth.Heal(3); // หรือชื่อฟังก์ชันฮีลที่ตั้มตั้งไว้ใน Health.cs
            ShowFloatingText("+3", Color.green);
        }

        RandomizeGreenZone(); // ย้ายที่โซนเขียว
        RandomizeSpeed(); // กวนตีนต่อด้วยสปีดใหม่
        // TODO: สั่งเพิ่มเลือดป้อมตรงนี้
    }

    void HandleFail()
    {
        missCount++;
        Debug.Log($"<color=red>พลาด!</color> สะสมแต้มพลาด: {missCount}/3");

        audioSource.PlayOneShot(failSound);

        // TODO: สั่งลดเลือดป้อมตรงนี้ (บทลงโทษกดพลาด)
        if (myTowerHealth != null)
        {
            myTowerHealth.TakeDamage(1); // หรือชื่อฟังก์ชันลดเลือดที่ตั้มตั้งไว้
            ShowFloatingText("-1", Color.red);
        }

        if (missCount >= 3)
        {
            StartCoroutine(LockoutRoutine());
        }
    }

    IEnumerator LockoutRoutine()
    {
        isLockout = true;
        missCount = 0;
        // เปิดป้ายแดง
        if (lockoutPanel != null) lockoutPanel.SetActive(true);

        Debug.Log("<color=orange>LOCKED OUT! รอ 5 วินาที...</color>");

        // ซ่อน UI หรือทำให้เป็นสีเทาตามใจชอบ
        // repairGroup.SetActive(false); 

        yield return new WaitForSeconds(5f);

        isLockout = false;
        missCount = 0;
        // repairGroup.SetActive(true);
        // ปิดป้ายแดง
        if (lockoutPanel != null) lockoutPanel.SetActive(false);
        Debug.Log("ปลดล็อกแล้ว! ซ่อมต่อได้");
    }

    // ฟังก์ชันเสริมสำหรับเสกตัวหนังสือ (พิมพ์เพิ่มไว้ล่างสุด)
    void ShowFloatingText(string message, Color textColor)
    {
        if (floatingTextPrefab != null && textSpawnPoint != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, textSpawnPoint.position, Quaternion.identity, textSpawnPoint);

            // +++ เปลี่ยนมาใช้ TextMeshProUGUI แทน Text ธรรมดา +++
            // ใช้ GetComponentInChildren เผื่อตั้มเอา Text ไปใส่ไว้ในลูกของ Prefab
            TextMeshProUGUI myText = textObj.GetComponentInChildren<TextMeshProUGUI>();

            if (myText != null)
            {
                myText.text = message;
                myText.color = textColor;
            }
            else
            {
                Debug.LogWarning("หา TextMeshPro ไม่เจอ! ตรวจสอบ Prefab ด่วน!");
            }
        }
    }
}