using UnityEngine;
using UnityEngine.UI;
public class TowerSlot : MonoBehaviour
{
    [Header("Setup")]
    public GameObject towerPrefab;
    public float cooldownTime = 5f; // คูลดาวน์ของเลนนี้ 5 วินาที

    [Header("UI Cooldown")]
    public Image cooldownOverlay; // ลากแผ่นสีดำ (Cooldown_Overlay) มาใส่ช่องนี้
    public GameObject slotUI;

    private GameObject currentTower;
    private SpriteRenderer spriteRenderer;
    private float nextBuildTime = 0f; // ตัวนับเวลาส่วนตัวของเลนนี้
    private bool isFirstTimePlaced = false; // เช็คว่าเลนนี้เคยถูกวางป้อมหรือยัง

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        // เช็คว่า: เลนนี้ไม่มีป้อมใช่มั้ย? AND เวลาปัจจุบันเกินเวลาคูลดาวน์แล้วใช่มั้ย?
        if (currentTower == null && Time.time >= nextBuildTime)
        {
            currentTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);

            // เริ่มนับเวลาคูลดาวน์ใหม่เฉพาะเลนนี้
            nextBuildTime = Time.time + cooldownTime;

            if (spriteRenderer != null) spriteRenderer.enabled = false;

            if (slotUI != null) slotUI.SetActive(false);

            // +++ ถ้านี่คือการวางป้อมครั้งแรกของเลนนี้ ให้ตะโกนบอก WaveManager +++
            if (!isFirstTimePlaced)
            {
                isFirstTimePlaced = true;
                if (WaveManager.instance != null)
                {
                    WaveManager.instance.TowerPlacedForFirstTime();
                }
            }
        }
        else if (Time.time < nextBuildTime)
        {
            Debug.Log($"เลนนี้ติด Cooldown! เหลืออีก {nextBuildTime - Time.time:F1} วินาที");
        }
    }

    void Update()
    {
        if (currentTower == null && spriteRenderer != null && !spriteRenderer.enabled)
        {
            spriteRenderer.enabled = true;
            if (slotUI != null && !slotUI.activeSelf) slotUI.SetActive(true);
        }

        if (cooldownOverlay != null)
        {
            if (Time.time < nextBuildTime)
            {
                // ถ้าติดคูลดาวน์อยู่: คำนวณเวลาที่เหลือหารด้วยเวลาทั้งหมด (จะเข้าสูตร 0.0 ถึง 1.0 พอดี)
                float timeRemaining = nextBuildTime - Time.time;
                cooldownOverlay.fillAmount = timeRemaining / cooldownTime;
            }
            else
            {
                // ถ้าพร้อมวางแล้ว: ให้แผ่นสีดำหายไป (Fill = 0)
                cooldownOverlay.fillAmount = 0;
            }
        }
    }
}