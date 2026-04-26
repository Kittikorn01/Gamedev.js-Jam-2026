using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // ใช้ถ้าตั้มใช้ TextMeshPro ที่ปุ่ม

public class TutorialManager : MonoBehaviour
{
    public GameObject[] pages; // ใส่ Page_1, 2, 3
    public GameObject backButton;
    public TextMeshProUGUI nextButtonText; // ข้อความบนปุ่ม Next

    private int currentPage = 0;

    void Start()
    {
        UpdateUI();
    }

    public void NextPage()
    {
        // ถ้าอยู่หน้าสุดท้ายแล้วกด Next -> ให้โหลดเข้าเกมเลย
        if (currentPage == pages.Length - 1)
        {
            SceneManager.LoadScene("GameScene"); // เปลี่ยนชื่อให้ตรงกับ Scene เกมจริง
            return;
        }

        currentPage++;
        UpdateUI();
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // เปิดปิดหน้า Page ให้ตรงกับ Index
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPage);
        }

        // ปิดปุ่ม Back ถ้าอยู่หน้าแรก
        backButton.SetActive(currentPage > 0);

        // เปลี่ยนคำบนปุ่ม Next ถ้าอยู่หน้าสุดท้าย
        if (currentPage == pages.Length - 1)
        {
            nextButtonText.text = "Start Game";
        }
        else
        {
            nextButtonText.text = "Next";
        }
    }
}