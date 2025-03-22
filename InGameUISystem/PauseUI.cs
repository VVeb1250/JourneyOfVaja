using TMPro;
using UnityEngine;

public class PauseUI : MyTransition {
    public TextMeshProUGUI[] menuList;
    public RectTransform Hilight;
    UIManager uiManager;

    [System.Obsolete]
    void Start()
    {
        currentIndex = 0;
        uiManager = FindObjectOfType<UIManager>();
    }

    [System.Obsolete]
    void Update()
    {
        ///////////////////////////////////
        // การควบคุมด้วยลูกศร
        if (Input.GetKeyDown(KeyCode.DownArrow)) // ชี้ลง
        {
            this.currentIndex = (this.currentIndex + 1) % menuList.Length;  // เลือกเมนูถัดไป
            Input.ResetInputAxes();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) // ชี้ขึ้น
        {
            this.currentIndex = (this.currentIndex - 1 + menuList.Length) % menuList.Length;  // เลือกเมนูก่อนหน้า
            Input.ResetInputAxes();
        }
        ///////////////////////////////////
        UpdateHighlightBox();
        if (Input.GetKeyDown(KeyCode.Z)) { SelectMenuItem(currentIndex); }
        if (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) {
            uiManager.ExitPauseUI();
        }
    }

    [System.Obsolete]
    public void SelectMenuItem(int index) {
        if (menuList[index].text == "Continue")
        {
            Debug.Log("เล่นต่อ");
            base.SetTransitionCooldown();
            uiManager.ExitPauseUI();
        } 
        else if (menuList[index].text == "Setting")
        {
            Debug.Log("ไปที่ตั้งค่า");
        } 
        else if (menuList[index].text == "To Main Menu")
        {
            Debug.Log("กลับไปหน้าหลัก");
            uiManager.ExitPauseUI();
            GameManager.Instance.LoadMap("MainMenu");
        }
    }
    private void UpdateHighlightBox()
    {
        if (menuList == null || currentIndex >= menuList.Length || Hilight == null)
        {
            Debug.LogWarning("Required components not initialized in UpdateHighlightBox");
            return;
        }

        Vector3 tempPos = menuList[currentIndex].transform.position;
        // tempPos.x -= 100;
        Hilight.transform.position = Vector3.Lerp(
            Hilight.transform.position,
            tempPos,
            TransitionSpeed * Time.unscaledDeltaTime * 4); // ใช้ unscaledDeltaTime
    }
    protected override void EnterTransition() { return; }
    protected override void ExitTransition() { return; }

    public void OnPointerEnter(int index) // ฟังก์ชั่นที่จะถูกเรียกเมื่อเมาส์ชี้ไปที่ตัวเลือก
    {
        currentIndex = index;
        UpdateHighlightBox();
    }

    [System.Obsolete]
    public void OnPointerClick(int index) // ฟังก์ชั่นที่จะถูกเรียกเมื่อเมาส์ชี้ไปที่ตัวเลือก
    {
        currentIndex = index;
        UpdateHighlightBox();
        SelectMenuItem(currentIndex);
    }
} 