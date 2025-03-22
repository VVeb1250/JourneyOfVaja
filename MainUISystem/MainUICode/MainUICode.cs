using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
// using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;  // ใช้สำหรับการจัดการ PointerEventData

public class MainUICode : MyTransition
{
    // ทำ panel
    public GameObject[] panels;  // Array ของ Panels ที่จะใช้
    public GameObject Heading;
    public float maxParallaxFactor = 0.5f;  // ค่าสูงสุดของ parallax effect
    public float distanceMultiplier = 0.5f;  // ปรับระยะห่างที่เพิ่มการเคลื่อนที่
    private Vector3 screenCenter;  // ตำแหน่งกลางของหน้าจอ
    private float halfscreenWidth;  // ความกว้างของหน้าจอ
    private float halfscreenHeight;  // ความสูงของหน้าจอ
    // ทำ select menu
    public TextMeshProUGUI[] menuItems;  // อาร์เรย์ของ TextMeshPro ที่ใช้ในเมนู
    public RectTransform Hilight;  // ตัวแสดงข้อความที่เลือก
    private Vector3[] oldPosition;
    private Vector3[] exitPosition;
    // bool for check setting
    private static bool isSelectSave = false;
    private static bool isSelectSetting = false;
    // get private att.
    public static bool getIsSelectSave() { return isSelectSave; }
    public static bool getIsSelectSetting() { return isSelectSetting; }

    void OnEnable() {
        isSelectSave = false;
        isSelectSetting = false;
        SelectSaveCode.setIsSelectSavePage(false);
    }
    void Start()
    {
        SetupMainMenu();
        CalculateCenterScreen(); // ทำ panel
        foreach (GameObject panel in panels) { panel.transform.position = screenCenter; }
    }
    private void SetupMainMenu() {
        // สร้างที่อยู่ของเมนูเมื่อกด save load
        exitPosition = new Vector3[panels.Length];
        // สร้างที่อยู่เก่าเพื่อ transion
        oldPosition = new Vector3[panels.Length + menuItems.Length + 1];
        int i = 0;
        foreach (GameObject panel in panels) { oldPosition[i] = panel.transform.position; i++; }
        foreach (TextMeshProUGUI menuItem in menuItems) { oldPosition[i] = menuItem.transform.position; i++; }
        oldPosition[i] = Heading.transform.position;

        UpdateHighlightBox();
    }
    private void CalculateCenterScreen() {
        // เริ่มต้นค่าของตำแหน่งกลางหน้าจอ
        halfscreenWidth = Screen.width / 2;
        halfscreenHeight = Screen.height / 2;
        screenCenter = new Vector3(halfscreenWidth, halfscreenHeight, 0);

        // ตั้ง Panel ให้เริ่มต้นที่กลางหน้าจอ (หรือ Canvas)
        foreach (GameObject panel in panels)
        {
            Vector3 centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenCenter.x, screenCenter.y, Camera.main.nearClipPlane));
            panel.transform.position = new Vector3(centerPosition.x, centerPosition.y, 0);  // ตั้งให้ Panel อยู่ตรงกลาง
        }
    }

    void Update()
    {
        if (!isSelectSave && !isSelectSetting && base.CheckTransitionCooldown()) {
            EnterTransition(); // transition กับ menu
            if (Input.GetKeyDown(KeyCode.Z)) { SelectMenuItem(currentIndex); }
            base.ListenerByKeys(menuItems.Length); // no need cuz always up date hilightbox on EnterTransition()
            CalculateMouseWithScreen(); // ทำ bg
        } else { 
            ExitTransition();
            CancelSelectSaveCancelCheck();
        }
    }
    public void SelectMenuItem(int index) {
        // ตัวอย่างเช่น หากเลือก "New Game"
        if (menuItems[index].text == "Play")
        {
            // ใส่คำสั่งการเริ่มเกมใหม่
            Debug.Log("เลือกเซฟ");
            SetLookUpTransition();
            base.SetTransitionCooldown();
            isSelectSave = true;
        }
        else if (menuItems[index].text == "Setting")
        {
            // ใส่คำสั่งเปิด Settings
            Debug.Log("ไปที่ตั้งค่า");
            SetLookUpTransition();
            base.SetTransitionCooldown();
            isSelectSetting = true;
        }
        else if (menuItems[index].text == "Quit")
        {
            // ใส่คำสั่งออกจากเกม
            Debug.Log("ออกจากเกม");
            Application.Quit();
        }
    }
    private void SetLookUpTransition() {
        // คำนวณค่าการเคลื่อนที่ (parallax) โดยใช้ระยะห่างจากกลางหน้าจอ
        float parallaxFactor = 0;
        int i = 0;
        foreach (GameObject panel in panels) {
            Vector3 cameraPosition = CalculateMouseWithScreen();
            cameraPosition.y -= parallaxFactor * (halfscreenHeight * 2);
            // คำนวณตำแหน่ง Y ของพื้นหลังที่จะเคลื่อนที่  panel.transform.position.y - (parallaxFactor * (halfscreenHeight * 2))
            exitPosition[i] = cameraPosition;
            // ตั้งค่าตาม parallel
            parallaxFactor += maxParallaxFactor;
            i += 1;
        }
    }
    protected override void EnterTransition()
    {
        int i = 0;
        foreach (GameObject panel in panels) { 
            panel.transform.position = Vector3.Lerp(panel.transform.position, oldPosition[i], TransitionSpeed * Time.deltaTime);
            i++; 
        }
        foreach (TextMeshProUGUI menuItem in menuItems) { 
            menuItem.transform.position = Vector3.Lerp(menuItem.transform.position, oldPosition[i], TransitionSpeed * Time.deltaTime);
            i++; 
        }
        Heading.transform.position = Vector3.Lerp(Heading.transform.position, oldPosition[i], TransitionSpeed * Time.deltaTime);
        UpdateHighlightBox();
    }
    protected override void ExitTransition()
    {
        for (int i = 0; i < panels.Length; i++) {
            // ใช้ Lerp เพื่อขยับอย่างนุ่มนวล
            panels[i].transform.position = Vector3.Lerp(panels[i].transform.position, exitPosition[i], TransitionSpeed * Time.deltaTime);
        }
        for (int i = 0; i < menuItems.Length; i++) {
            // ใช้ Lerp เพื่อขยับอย่างนุ่มนวล
            menuItems[i].transform.position = Vector3.Lerp(menuItems[i].transform.position, exitPosition[^1], TransitionSpeed * Time.deltaTime);
        }
        Heading.transform.position = Vector3.Lerp(Heading.transform.position, exitPosition[^1], TransitionSpeed * Time.deltaTime);
        Hilight.transform.position = Vector3.Lerp(Hilight.transform.position, exitPosition[^1], TransitionSpeed * Time.deltaTime);
    }
    private void CancelSelectSaveCancelCheck() {
        if (isSelectSave)
        {
            if (Input.GetKeyDown(KeyCode.X)) { isSelectSave = false; }
            if (Input.GetMouseButtonDown(1)) { isSelectSave = false; }
        }
        if (isSelectSetting)
        {
            if (Input.GetKeyDown(KeyCode.X)) { isSelectSetting = false; }
            if (Input.GetMouseButtonDown(1)) { isSelectSetting = false; }
        }
    }
    private void UpdateHighlightBox()
    {
        // เริ่มต้น Coroutine สำหรับการเคลื่อนที่ของกล่องไฮไลท์
        Hilight.transform.position = Vector3.Lerp(
            Hilight.transform.position, 
            menuItems[currentIndex].transform.position, 
            TransitionSpeed * Time.deltaTime * 4);
    }
    private Vector3 CalculateMouseWithScreen() {
                // รับตำแหน่งของเมาส์ในหน้าจอ
        Vector3 mousePosition = Input.mousePosition;

        // คำนวณระยะห่างจากตำแหน่งกลางของหน้าจอ
        Vector3 deltaMouse = mousePosition - screenCenter;

        // คำนวณค่าการเคลื่อนที่ (parallax) โดยใช้ระยะห่างจากกลางหน้าจอ
        float parallaxFactor = maxParallaxFactor;

        // แปลงจาก screen space เป็น world space
        Vector3 movementInWorldSpace = Camera.main.ScreenToWorldPoint(deltaMouse);
        movementInWorldSpace.z = 0;  // เราต้องการให้เคลื่อนที่ใน 2D (แกน Z คงที่)

        // การจำกัดขอบเขตการเคลื่อนที่ของเมาส์
        movementInWorldSpace.x = Mathf.Clamp(movementInWorldSpace.x, -halfscreenWidth, halfscreenWidth);
        movementInWorldSpace.y = Mathf.Clamp(movementInWorldSpace.y, -halfscreenHeight, halfscreenHeight);

        // ขยับ panels ทุกอันตามค่าการเคลื่อนที่ที่ปรับตาม parallax
        foreach (GameObject panel in panels)
        {
            // การเคลื่อนที่ไปในทิศทางตรงข้ามกับเมาส์ (Inverse Parallax)
            // ตั้งค่าการเคลื่อนที่ให้เป็นไปตาม Parallax Effect ที่คำนวณแล้ว
            Vector3 newposition = (movementInWorldSpace * (parallaxFactor * distanceMultiplier)) + screenCenter;
            panel.transform.position = Vector3.Lerp(panel.transform.position, newposition, TransitionSpeed * Time.deltaTime);
            parallaxFactor += parallaxFactor;
        }
        return movementInWorldSpace + screenCenter;
    }
    // add listener
    public void OnPointerEnter(int index) // ฟังก์ชั่นที่จะถูกเรียกเมื่อเมาส์ชี้ไปที่ตัวเลือก
    {
        currentIndex = index;
        UpdateHighlightBox();
    }
    public void OnPointerClick(int index) // ฟังก์ชั่นที่จะถูกเรียกเมื่อเมาส์ชี้ไปที่ตัวเลือก
    {
        currentIndex = index;
        UpdateHighlightBox();
        SelectMenuItem(currentIndex);
    }

}