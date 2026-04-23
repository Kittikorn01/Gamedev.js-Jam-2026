using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [Header("Cooldown Settings")]
    public float buildCooldown = 5f; // วางป้อมได้ 1 ครั้ง ทุกๆ 5 วินาที
    private float nextBuildTime = 0f;

    // ฟังก์ชันให้ Slot ถามว่า "ตอนนี้วางป้อมได้ยังลูกพี่?"
    public bool CanBuild()
    {
        return Time.time >= nextBuildTime;
    }

    // ฟังก์ชันให้ Slot สั่ง "หนูวางป้อมแล้วนะลูกพี่ เริ่มจับเวลาคูลดาวน์เลย!"
    public void StartCooldown()
    {
        nextBuildTime = Time.time + buildCooldown;
        Debug.Log($"<color=cyan>เริ่ม Cooldown วางป้อม {buildCooldown} วินาที</color>");
    }
}