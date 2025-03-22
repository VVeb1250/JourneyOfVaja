using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectSaveSlot : MyTransition, ISelectSave
{
    // import prefab for multiple save icon
    public GameObject[] childSaveItem;
    public Transform containerParent;       // Parent ที่จะใช้สำหรับจัดตำแหน่งการแสดงผล
    public GameObject saveItem; // import
    protected Color originalColor;             // สีเดิมที่ใช้อยู่
    protected Color newColor = new(0.3283019f/2, 0.2452019f/2, 0.1257458f/2);
    public int xSpacing = 700;
    public int verticalSpacing = 175;
    public int savehave = 4;
    // header
    public TextMeshProUGUI Header;
    private TextMeshProUGUI[] inHeader;
    // make transition
    protected Vector3[] oldSaveItemPosition;
    protected Vector3 oldHeaderPosition;
    protected float selectSaveTransitionTime = 5;
    protected static bool isSelectSaveSlot = false;
    public static bool getIsSelectSaveSlot() { return isSelectSaveSlot; }
    public static void setIsSelectSaveSlot(bool setter) { isSelectSaveSlot = setter; }
    // ข้อมูลการเซฟ (ตัวอย่าง)
    protected string[] saveIDs = { "Save 1", "Save 2", "Save 3", "Save 4" }; 
    protected string[] saveDetails = { "Forest Of Navana", "Maneval Cliff", "Danareval Cave", "Empty" };
    protected string[] saveDates = { "(15:29:23) 2025-02-18", "(02:57:14) 2025-02-17", "(22:01:45) 2025-02-16", "" };
    public string[][] getSave() { return new string[][] { saveIDs, saveDetails, saveDates }; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Obsolete]
    void Start()
    {
        SetupOldPosition();
        CreateSaveContainers();
        LoadSaveData();        // Load data first
        UpdateSaveListSelection();
        inHeader = Header.GetComponentsInChildren<TextMeshProUGUI>();
    }
    protected void SetupOldPosition() {
        oldHeaderPosition = Header.transform.position;
    }
    public void LoadSaveData()
    {
        Debug.Log("Loading save data...");
        try
        {
            // Initialize arrays with correct size if not already initialized
            if (saveIDs == null || saveIDs.Length != savehave)
            {
                saveIDs = new string[savehave];
                saveDetails = new string[savehave];
                saveDates = new string[savehave];
            }

            for (int i = 0; i < savehave; i++)
            {
                // Initialize default values
                saveIDs[i] = $"Save {i + 1}";
                saveDetails[i] = "Empty";
                saveDates[i] = "";

                SaveData data = SaveManager.Instance.LoadGame(i);
                if (data != null)
                {
                    if (!data.isEmpty)
                    {
                        saveIDs[i] = data.saveID;
                        saveDetails[i] = data.sceneName;
                        saveDates[i] = $"({data.playTime}) {data.lastPlayedDate}";
                    }
                    Debug.Log($"Loaded save {i}: {saveIDs[i]}, {saveDetails[i]}, {saveDates[i]}");

                    // Update UI if containers exist
                    UpdateSaveContainer(i);
                }
                else
                {
                    Debug.LogWarning($"No save data found for slot {i}, using default values");
                    UpdateSaveContainer(i);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading save data: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    private void UpdateSaveContainer(int index)
    {
        if (childSaveItem != null && childSaveItem[index] != null)
        {
            TextMeshProUGUI[] textComponents = childSaveItem[index].GetComponentsInChildren<TextMeshProUGUI>();
            if (textComponents != null && textComponents.Length >= 3)
            {
                textComponents[0].text = saveIDs[index];
                textComponents[1].text = saveDetails[index];
                textComponents[2].text = saveDates[index];
                Debug.Log($"Updated container {index} with {saveIDs[index]}");
            }
        }
    }

    public void CreateSaveContainers()
    {
        // First-time creation of containers
        oldSaveItemPosition = new Vector3[savehave];
        oldSaveItemPosition[0] = containerParent.transform.position;
        for (int i = 1; i < savehave; i++)
        {
            Vector2 vector = containerParent.transform.position;
            vector.y -= i * verticalSpacing;
            oldSaveItemPosition[i] = vector;
            childSaveItem[i].transform.position = oldSaveItemPosition[i];
        }

        for (int i = 0; i < savehave; i++)
        {
            // Create container
            childSaveItem[i].name = $"SaveContainer_{i}";

            Vector3 position = childSaveItem[i].transform.position;
            position.x += 2000;
            childSaveItem[i].transform.position = position;
            
            // Set up RectTransform position
            RectTransform rectTransform = childSaveItem[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -i * verticalSpacing);

            // Set up text components
            TextMeshProUGUI[] textComponents = childSaveItem[i].GetComponentsInChildren<TextMeshProUGUI>();
            if (i == 0) originalColor = textComponents[0].color;

            // Add container component
            SaveContainerV2 saveContainer = childSaveItem[i].AddComponent<SaveContainerV2>();
            saveContainer.Initialize(i, this);

            // Update container content
            UpdateSaveContainer(i);

            Debug.Log($"Created save container {i}");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (SavePage.isLoad) { inHeader[1].text = "Select Load"; }
        else { inHeader[1].text = "Select Save"; }

        if (RestUICode.isSelectSave && !isSelectSaveSlot && base.CheckTransitionCooldown()) {
            if (Input.GetKeyDown(KeyCode.Z)) { SelectSaveItem(currentIndex); }
            base.ListenerByKeys(childSaveItem.Length);

            if (saveDetails[currentIndex] == "Empty" && SavePage.isLoad) { 
                this.currentIndex = (this.currentIndex + 1) % childSaveItem.Length; 
            } // เลือกเมนูถัดไปเมื่อว่าง

            UpdateSaveListSelection();
            EnterTransition();
        } else 
        { 
            ExitTransition(); 
            CancelSelectSaveCancelCheck(); 
        }
        if (RestUICode.isSelectSave && (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(1))) 
        { 
            RestUICode.isSelectSave = false;
            base.SetTransitionCooldown();
        }
    }
    public void SelectSaveItem(int index) {
        if (saveDetails[index] == "Empty" && SavePage.isLoad) { return; }

        ISelectSave.selectedData = index;
        SavePage.canDeleteSave = false;

        isSelectSaveSlot = true;
        base.SetTransitionCooldown();
        Debug.Log(saveIDs[currentIndex]);
    }
    protected void CancelSelectSaveCancelCheck() {
        if (isSelectSaveSlot)
        {
            if (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(1)) 
            { 
                LoadSaveData();
                CreateSaveContainers();
                isSelectSaveSlot = false;
                base.SetTransitionCooldown();
            }
        }
    }
    protected void UpdateSaveListSelection()
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

class SaveContainerV2 : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    protected SelectSaveSlot selectSaveSlot;
    protected int index;

    // ฟังก์ชั่นนี้จะถูกเรียกจาก SelectSaveCode เพื่อผูก index และ SelectSaveCode
    public void Initialize(int index, SelectSaveSlot selectSaveSlot)
    {
        this.index = index;
        this.selectSaveSlot = selectSaveSlot;
    }

    // เมื่อเมาส์เข้าไปในพื้นที่ของ SaveContainer จะเรียกฟังก์ชั่นนี้
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ส่ง index ที่ถูกเลือกไปยัง selectSaveSlot
        selectSaveSlot.OnPointerEnter(index);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // ส่ง index ที่ถูกเลือกไปยัง selectSaveSlot
        selectSaveSlot.SelectSaveItem(index);
    }

}