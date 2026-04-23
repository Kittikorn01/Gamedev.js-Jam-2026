using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [Header("Setup")]
    public GameObject towerPrefab;
    public float cooldownTime = 5f; // คูลดาวน์ของเลนนี้ 5 วินาที

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
        }
    }
}