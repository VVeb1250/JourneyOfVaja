using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectSaveCode : MyTransition
{
    // import main UI for check
    private MainUICode mainUI;
    // import prefab for multiple save icon
    private GameObject[] childSaveItem;
    public Transform containerParent;       // Parent ที่จะใช้สำหรับจัดตำแหน่งการแสดงผล
    public GameObject saveItem; // import
    private Color originalColor;             // สีเดิมที่ใช้อยู่
    private Color newColor = new(0.3283019f/2, 0.2452019f/2, 0.1257458f/2);
    public int xSpacing = 500;
    public int verticalSpacing = 175;
    public int savehave = 4;
    // header
    public TextMeshProUGUI Header;
    // make transition
    private Vector3[] oldSaveItemPosition;
    private Vector3 oldHeaderPosition;
    private float selectSaveTransitionTime = 5;
    private bool isSelectSavePage = false;
    public bool getIsSelectSavePage() { return isSelectSavePage; }
    public void setIsSelectSavePage(bool isSelectSavePage) { this.isSelectSavePage = isSelectSavePage; }
    // ข้อมูลการเซฟ (ตัวอย่าง)
    private int saveSelected;
    public int getSaveSelected() { return this.saveSelected; }
    private string[] saveIDs = { "Save 1", "Save 2", "Save 3", "Save 4" }; 
    private string[] saveDetails = { "Forest Of Navana", "Maneval Cliff", "Danareval Cave", "Empty" };
    private string[] saveDates = { "(15:29:23) 2025-02-18", "(02:57:14) 2025-02-17", "(22:01:45) 2025-02-16", "" };
    public string[][] getSave() { return new string[][] { saveIDs, saveDetails, saveDates }; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [System.Obsolete]
    void Start()
    {
        mainUI = FindObjectOfType<MainUICode>();  // เพิ่ม MainUICode
        // ตรวจสอบว่าตั้งค่าเสร็จแล้ว และกำหนด SelectSaveCode
        StartCoroutine(WaitForMainUICodeSetup());
        SetupOldPosition();
        CreateSaveContainers();
        UpdateSaveListSelection();
    }
    private IEnumerator WaitForMainUICodeSetup()     // Coroutine เพื่อตรวจสอบว่า MainUICode ถูกตั้งค่าแล้ว
    {
        // รอให้ MainUICode ถูกตั้งค่า
        yield return new WaitUntil(() => mainUI.panels != null);
    }
    private void SetupOldPosition() {
        oldHeaderPosition = Header.transform.position;
    }
    void CreateSaveContainers()
    {
        childSaveItem = new GameObject[savehave];
        oldSaveItemPosition = new Vector3[savehave];
        // สร้าง SaveContainer ตามจำนวนข้อมูลที่มี
        for (int i = 0; i < childSaveItem.Length; i++)
        {
            // Instantiate Prefab ของ SaveContainer
            childSaveItem[i] = Instantiate(saveItem, containerParent);
            oldSaveItemPosition[i] = containerParent.transform.position;
            oldSaveItemPosition[i].x -= i * (verticalSpacing * 2);
            oldSaveItemPosition[i].y -= i * verticalSpacing;

            // หา TextMeshProUGUI ใน SaveContainer
            TextMeshProUGUI[] textComponents = childSaveItem[i].GetComponentsInChildren<TextMeshProUGUI>();
            originalColor = textComponents[0].color;

            if (textComponents.Length >= 3)
            {
                // เปลี่ยนข้อความใน TextMeshProUGUI
                textComponents[0].text = saveIDs[i];        // SaveID
                textComponents[1].text = saveDetails[i];    // SaveDetail
                textComponents[2].text = saveDates[i];      // SaveDate
            }

            // ปรับตำแหน่งของ SaveContainer แต่ละตัวให้ไล่ลงมา
            RectTransform rectTransform = childSaveItem[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-i * (verticalSpacing * 2), -i * verticalSpacing);

            // สร้างและผูก SaveContainer กับ childSaveItem
            SaveContainer saveContainer = childSaveItem[i].AddComponent<SaveContainer>();
            saveContainer.Initialize(i, this); // ส่ง index และ reference ไปยัง SelectSaveCode
        }
    }
    // Update is called once per frame
    void Update()
    {
        Header.text = "Select Save";

        if (mainUI.getIsSelectSave() && !isSelectSavePage && base.CheckTransitionCooldown()) {
            if (Input.GetKeyDown(KeyCode.Z)) { SelectSaveItem(currentIndex); }
            base.ListenerByKeys(childSaveItem.Length); { UpdateSaveListSelection(); }
            EnterTransition();
        } else 
        { 
            ExitTransition(); 
            CancelSelectSaveCancelCheck(); 
        }
    }
    public void SelectSaveItem(int index) {
        saveSelected = index;
        isSelectSavePage = true;
        base.SetTransitionCooldown();
        Debug.Log(saveIDs[currentIndex]);
    }
    private void CancelSelectSaveCancelCheck() {
        if (isSelectSavePage)
        {
            if (Input.GetKeyDown(KeyCode.X)) { isSelectSavePage = false; }
            if (Input.GetMouseButtonDown(1)) { isSelectSavePage = false; }
            base.SetTransitionCooldown();
        }
    }
    private void UpdateSaveListSelection()
    {
        // รีเซ็ตสถานะการเลือกทั้งหมด
        foreach (GameObject child in childSaveItem) {
            TextMeshProUGUI[] textComponents = child.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI textComponent in textComponents) {
                textComponent.color = originalColor;
            }   
        }
        // ตัวเลือกนี้ถูกเลือก
        TextMeshProUGUI[] textComponents2 = childSaveItem[currentIndex].GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textComponent in textComponents2) {
            textComponent.color = newColor;
        }   
    }
    protected override void EnterTransition() {
        Vector3 newHeaderPosition = oldHeaderPosition;
        newHeaderPosition.y -= 500;
        Header.transform.position = Vector3.Lerp(Header.transform.position, newHeaderPosition, selectSaveTransitionTime * Time.deltaTime);
        
        for (int i = 0; i < childSaveItem.Length; i++) {
            Vector3 newChildSaveItemPosition = oldSaveItemPosition[i];
            if (i == currentIndex) { newChildSaveItemPosition.x = xSpacing + 100; }
            else { newChildSaveItemPosition.x = xSpacing; }
            childSaveItem[i].transform.position = Vector3.Lerp(childSaveItem[i].transform.position, newChildSaveItemPosition, selectSaveTransitionTime * Time.deltaTime);
        }
    }
    protected override void ExitTransition() {
        Header.transform.position = Vector3.Lerp(Header.transform.position, oldHeaderPosition, selectSaveTransitionTime * Time.deltaTime);

        for (int i = 0; i < childSaveItem.Length; i++) {
            childSaveItem[i].transform.position = Vector3.Lerp(childSaveItem[i].transform.position, oldSaveItemPosition[i], selectSaveTransitionTime * Time.deltaTime);
        }
    }

    public void OnPointerEnter(int index) // ฟังก์ชั่นที่จะถูกเรียกเมื่อเมาส์ชี้ไปที่ตัวเลือก
    {
        currentIndex = index;
        UpdateSaveListSelection();
    }
}

class SaveContainer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private SelectSaveCode selectSaveCode;
    private int index;

    // ฟังก์ชั่นนี้จะถูกเรียกจาก SelectSaveCode เพื่อผูก index และ SelectSaveCode
    public void Initialize(int index, SelectSaveCode selectSaveCode)
    {
        this.index = index;
        this.selectSaveCode = selectSaveCode;
    }

    // เมื่อเมาส์เข้าไปในพื้นที่ของ SaveContainer จะเรียกฟังก์ชั่นนี้
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ส่ง index ที่ถูกเลือกไปยัง SelectSaveCode
        selectSaveCode.OnPointerEnter(index);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // ส่ง index ที่ถูกเลือกไปยัง SelectSaveCode
        selectSaveCode.SelectSaveItem(index);
    }
}