using System.Collections; // ต้องมีบรรทัดนี้เพื่อใช้ Coroutine (IEnumerator)
using UnityEngine;

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

    // Instance แบบ Singleton (เพื่อให้ศัตรูที่เพิ่งเกิด สามารถวิ่งมาหา Manager เพื่อรายงานตัวตอนตายได้ง่ายๆ)
    public static WaveManager instance;

    void Awake()
    {
        instance = this; // ประกาศตัวว่าเป็นผู้จัดการใหญ่
    }

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);

        
    }

    // 1. ระบบเสกมอนสเตอร์ (ทำงานแยกเป็นอิสระ ไม่กวน Update)
    IEnumerator SpawnEnemyRoutine()
    {
        // ทำซ้ำไปเรื่อยๆ จนกว่าจะเสกครบ 10 ตัว
        while (enemiesSpawned < totalEnemies)
        {
            // สุ่มเลน (ดึงจุดเกิดแบบสุ่มจาก Array)
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform randomSpawnPoint = spawnPoints[randomIndex];

            // เสกมอนสเตอร์ตรงจุดที่สุ่มได้
            Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

            enemiesSpawned++; // นับว่าเสกไปแล้ว 1 ตัว

            // รอเวลา 3 วินาที (หรือตามค่า spawnDelay) ค่อยวนลูปเสกตัวต่อไป
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // 2. ฟังก์ชันให้ศัตรูมาเรียกตอนที่มันตาย
    public void EnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"<color=green>ศัตรูตาย!</color> กำจัดไปแล้ว: {enemiesKilled}/{totalEnemies}");

        // ตรวจสอบเงื่อนไขชนะ
        if (enemiesKilled >= totalEnemies)
        {
            TriggerWinCondition();
        }
    }

    void TriggerWinCondition()
    {
        Debug.Log("<color=yellow>STAGE CLEAR! YOU WIN!</color>");

        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f; // หยุดเกม
    }

    // เพิ่มฟังก์ชันใหม่ให้ BaseHealth มาเรียกตอนมอนสเตอร์หลุดเข้าบ้าน
    public void EnemyEscaped()
    {
        enemiesKilled++; // ใช้ตัวแปรเดิมนับรวมไปเลย จะได้ไม่ซับซ้อน
        Debug.Log($"<color=orange>มอนสเตอร์หนีเข้าบ้าน!</color> จัดการไปแล้ว: {enemiesKilled}/{totalEnemies}");

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
}