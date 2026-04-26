using UnityEngine;
using UnityEngine.SceneManagement; // ตัวนี้ขาดไม่ได้!

public class SceneController : MonoBehaviour
{
    // ฟังก์ชันเปลี่ยน Scene (แค่พิมพ์ชื่อ Scene เข้าไป)
    public void LoadGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ฟังก์ชันเปิด/ปิด Panel (เอาไว้ใช้กับหน้า Credit)
    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            bool isActive = panel.activeSelf;
            panel.SetActive(!isActive);
        }
    }
}