using System.Collections; // ต้องมีบรรทัดนี้เพื่อใช้ Coroutine (IEnumerator)
using TMPro;
using UnityEngine;
using UnityEngine.UI; // ต้องมีบรรทัดนี้เพื่อใช้ UI (เช่น Slider)
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject enemyPrefab; // ลากมอนสเตอร์มาใส่
    public Transform[] spawnPoints; // ลากจุดเกิดทั้ง 3 เลนมาใส่ (เดี๋ยวสอนวิธีใส่ใน Unity)
    public float spawnDelay = 3f; // ให้มอนสเตอร์เกิดทุกๆ 3 วินาที
    public int totalEnemies = 10; // ด่านนี้มีศัตรูทั้งหมด 10 ตัว

    [Header("Progress Tracking")]
    private int enemiesSpawned = 0; // นับตัวที่เกิดมาแล้ว
    private int enemiesKilled = 0;  // นับตัวที่ตายแล้ว

    [Header("UI Elements")]
    public GameObject winPanel;

    [Header("Game Start Condition")]
    public int readyLanes = 0; // นับว่าวางป้อมไปกี่เลนแล้ว

    [Header("UI Progress")]
    public Slider waveSlider; // ลาก Slider จากหน้าจอมาใส่ช่องนี้

    [Header("End Game Stats UI")]
    public TextMeshProUGUI defeatedText;
    public TextMeshProUGUI escapedText;

    private List<int> laneBag = new List<int>();

    // สร้างตะกร้าแยกนับ (ถ้ายังไม่ได้แยก)
    public int deadCount = 0;
    public int escapedCount = 0;

    // Instance แบบ Singleton (เพื่อให้ศัตรูที่เพิ่งเกิด สามารถวิ่งมาหา Manager เพื่อรายงานตัวตอนตายได้ง่ายๆ)
    public static WaveManager instance;

    void Awake()
    {
        instance = this; // ประกาศตัวว่าเป็นผู้จัดการใหญ่
    }

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);

        if (waveSlider != null)
        {
            waveSlider.minValue = 0;
            waveSlider.maxValue = 1;
            waveSlider.value = 0;
        }
    }

    // 1. ระบบเสกมอนสเตอร์ (ทำงานแยกเป็นอิสระ ไม่กวน Update)
    IEnumerator SpawnEnemyRoutine()
    {
        int rushThreshold = Mathf.RoundToInt(totalEnemies * 0.6f);

        while (enemiesSpawned < totalEnemies)
        {
            if (enemiesSpawned == rushThreshold)
            {
                spawnDelay = 1.2f; // เข้าโหมด Rush
            }

            // *** เปลี่ยนมาใช้ฟังก์ชันถุงสุ่มแทน Random.Range ทื่อๆ ***
            int fairLaneIndex = GetFairRandomLane();
            Transform randomSpawnPoint = spawnPoints[fairLaneIndex];

            Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
            enemiesSpawned++;

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    int GetFairRandomLane()
    {
        // ถ้าถุงว่าง ให้เติมป้ายเลน 0, 1, 2 กลับเข้าไปใหม่ (ใส่ 2 ชุดเลยก็ได้ 0,1,2,0,1,2)
        if (laneBag.Count == 0)
        {
            laneBag.Add(0); laneBag.Add(1); laneBag.Add(2);
            laneBag.Add(0); laneBag.Add(1); laneBag.Add(2);
        }

        // สุ่มเลือกป้าย 1 ใบจากถุง
        int randomIndex = Random.Range(0, laneBag.Count);
        int chosenLane = laneBag[randomIndex];

        // หยิบแล้วเอาออกจากถุง
        laneBag.RemoveAt(randomIndex);

        return chosenLane;
    }
    // 2. ฟังก์ชันให้ศัตรูมาเรียกตอนที่มันตาย
    public void EnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"<color=green>ศัตรูตาย!</color> กำจัดไปแล้ว: {enemiesKilled}/{totalEnemies}");

        UpdateWaveProgress();

        deadCount++;

        // ตรวจสอบเงื่อนไขชนะ
        if (enemiesKilled >= totalEnemies)
        {
            TriggerWinCondition();
        }
    }

    void TriggerWinCondition()
    {
        Debug.Log("<color=yellow>STAGE CLEAR! YOU WIN!</color>");

        if (defeatedText != null) defeatedText.text = "Enemies Defeated: " + deadCount;
        if (escapedText != null) escapedText.text = "Enemies Escaped: " + escapedCount;

        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f; // หยุดเกม
    }

    // เพิ่มฟังก์ชันใหม่ให้ BaseHealth มาเรียกตอนมอนสเตอร์หลุดเข้าบ้าน
    public void EnemyEscaped()
    {
        enemiesKilled++; // ใช้ตัวแปรเดิมนับรวมไปเลย จะได้ไม่ซับซ้อน
        Debug.Log($"<color=orange>มอนสเตอร์หนีเข้าบ้าน!</color> จัดการไปแล้ว: {enemiesKilled}/{totalEnemies}");

        escapedCount++;

        UpdateWaveProgress();

        // เช็คชนะเหมือนเดิม (ถ้าหลุดเข้าบ้านแล้วเลือดบ้านยังไม่หมด ก็ถือว่ารอด Wave นี้)
        if (enemiesKilled >= totalEnemies)
        {
            TriggerWinCondition();
        }
    }

    // ให้ TowerSlot มาเรียกฟังก์ชันนี้ตอนวางป้อมครั้งแรก
    public void TowerPlacedForFirstTime()
    {
        readyLanes++;
        if (readyLanes >= 3)
        {
            Debug.Log("<color=green>วางครบ 3 เลนแล้ว! มอนสเตอร์บุกได้!</color>");
            StartCoroutine(SpawnEnemyRoutine()); // สั่งมอนสเตอร์เริ่มเกิดตรงนี้!
        }
    }

    public void UpdateWaveProgress()
    {
        if (waveSlider != null)
        {
            // คำนวณค่าเป็น 0.0 - 1.0
            float progress = (float)enemiesKilled / totalEnemies;
            waveSlider.value = progress;

            Debug.Log($"Wave Progress: {progress * 100}%");
        }
    }
}