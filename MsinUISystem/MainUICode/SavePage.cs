using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SavePage : MyTransition
{
    private SelectSaveCode selectSaveCode;
    public GameObject savePage;
    private TextMeshProUGUI[] textItems;  // อาร์เรย์ของ TextMeshPro ที่ใช้ในเมนู
    public RectTransform Hilight;  // ตัวแสดงข้อความที่เลือก
    private Vector3 oldPagePosition;
    private int index = 2;
    private string[][] saveData;
    // private string[] saveIDs = { "Save 1", "Save 2", "Save 3", "Save 4" }; 
    // private string[] saveDetails = { "Forest Of Navana", "Maneval Cliff", "Danareval Cave", "Empty" };
    // private string[] saveDates = { "(15:29:23) 2025-02-18", "(02:57:14) 2025-02-17", "(22:01:45) 2025-02-16", "" };
    private int selectedData;

    [System.Obsolete]
    void Start()
    {
        selectSaveCode = FindObjectOfType<SelectSaveCode>();  // เพิ่ม MainUICode
        textItems = savePage.GetComponentsInChildren<TextMeshProUGUI>();

        oldPagePosition = savePage.transform.position;
        saveData = selectSaveCode.getSave();
        UpdateSavePage();

        PageContainer pageContainer = textItems[4].AddComponent<PageContainer>();
        pageContainer.Initialize(0, this); // ส่ง index และ reference ไปยัง SelectSaveCode
        pageContainer = textItems[5].AddComponent<PageContainer>();
        pageContainer.Initialize(1, this); // ส่ง index และ reference ไปยัง SelectSaveCode

        UpdateHighlightBox();
    }
    private void UpdateSavePage()
    {
        selectedData = selectSaveCode.getSaveSelected();

        textItems[0].text = saveData[0][selectedData];
        textItems[2].text = saveData[1][selectedData];
        if (saveData[2][selectedData] == "") { 
            textItems[1].text = "";
            textItems[3].text = "";
        }
        else {
            textItems[1].text = "Playtime " + saveData[2][selectedData].Substring(1, 8);
            textItems[3].text = saveData[2][selectedData][11..];
        }
    }

    void Update()
    {
        if (selectSaveCode.getIsSelectSavePage() && base.CheckTransitionCooldown())
        {
            EnterTransition();
            UpdateSavePage();
            base.ListenerByLeftRight(index);
            if (Input.GetKeyDown(KeyCode.Z)) { SelectAction(currentIndex); }
        } else { ExitTransition(); }
        UpdateHighlightBox();
    }
    protected override void EnterTransition() {
        Vector3 newPagePosition = oldPagePosition;
        newPagePosition.x -= 2000;
        savePage.transform.position = Vector3.Lerp(savePage.transform.position, newPagePosition, TransitionSpeed * Time.deltaTime);
    }
    protected override void ExitTransition() {
        savePage.transform.position = Vector3.Lerp(savePage.transform.position, oldPagePosition, TransitionSpeed * Time.deltaTime);
    }
    private void UpdateHighlightBox()
    {
        Vector3 newPosition = textItems[currentIndex + 4].transform.position;
        // newPosition.x -= 2000;
        // เริ่มต้น Coroutine สำหรับการเคลื่อนที่ของกล่องไฮไลท์
        Hilight.transform.position = Vector3.Lerp(
            Hilight.transform.position, 
            newPosition, 
            TransitionSpeed * Time.deltaTime * 4);
    }
    public void SelectAction(int index)
    {
        if (index == 0) {

        } else if (index == 1) {
            selectSaveCode.setIsSelectSavePage(false);
        } base.SetTransitionCooldown();
    }

    public void OnPointerEnter(int index) // ฟังก์ชั่นที่จะถูกเรียกเมื่อเมาส์ชี้ไปที่ตัวเลือก
    {
        currentIndex = index;
        UpdateHighlightBox();
    }
}

class PageContainer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private SavePage savePage;
    private int index;

    // ฟังก์ชั่นนี้จะถูกเรียกจาก SelectSaveCode เพื่อผูก index และ SelectSaveCode
    public void Initialize(int index, SavePage savePage)
    {
        this.index = index;
        this.savePage = savePage;
    }

    // เมื่อเมาส์เข้าไปในพื้นที่ของ SaveContainer จะเรียกฟังก์ชั่นนี้
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ส่ง index ที่ถูกเลือกไปยัง pageContainer
        savePage.OnPointerEnter(index);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // ส่ง index ที่ถูกเลือกไปยัง pageContainer
        savePage.SelectAction(index);
    }
}