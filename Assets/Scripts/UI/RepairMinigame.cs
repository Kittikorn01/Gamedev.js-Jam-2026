using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    private float currentSpeed;
    private float progress = 0f; // record the current position of the needle (0 to 1)
    private int direction = 1;   // 1 for moving right, -1 for moving left
    private int missCount = 0;
    private bool isLockout = false;
    private float targetMin;
    private float targetMax;
    private bool movingForward = true; // ãªéàªç¤·ÔÈ·Ò§à¢çÁ

    void Start()
    {
        RandomizeSpeed();
        RandomizeGreenZone();
    }

    void Update()
    {
        if (isLockout) return;

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
        // 1. ÊØèÁ¤èÒ¢Íº«éÒÂÊØ´ (ÊØèÁµÑé§áµè 0 ä»¨¹¶Ö§¨Ø´·Õèà¼×èÍ¤ÇÒÁ¡ÇéÒ§äÇéáÅéÇ ¨Ðä´éäÁè·ÐÅØ¢ÇÒ)
        targetMin = Random.Range(0f, 1f - zoneWidth);

        // 2. ¤Ó¹Ç³¢Íº¢ÇÒÊØ´
        targetMax = targetMin + zoneWidth;

        // 3. ÊÑè§ÂéÒÂµÓáË¹è§ÀÒ¾â«¹ÊÕà¢ÕÂÇã¹ UI (ãªé·èÒäÁéµÒÂ Anchors)
        if (greenZone != null)
        {
            // µÑé§¨Ø´ÂÖ´«éÒÂ (Min) áÅÐ¢ÇÒ (Max) µÒÁ¤èÒ·ÕèàÃÒÊØèÁä´é
            greenZone.anchorMin = new Vector2(targetMin, 0);
            greenZone.anchorMax = new Vector2(targetMax, 1);

            // ÃÕà«çµ¢ÍºÀÒ¾ãËé¾Í´Õ¡Ñº¨Ø´ÂÖ´à»êÐæ
            greenZone.offsetMin = Vector2.zero; // Åº¢ÂÐ¤èÒ¾Ô¡à«Åà´ÔÁ·Ôé§
            greenZone.offsetMax = Vector2.zero;
        }
    }
}