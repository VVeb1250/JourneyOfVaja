using System;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; 

public interface ISelectSave { 
    public static int selectedData;
}

public class SavePage : MyTransition, ISelectSave
{
    private MainUICode mainUICode;
    private SelectSaveCode selectSaveCode;
    private SelectSaveSlot selectSaveSlot;
    public GameObject savePage;
    private TextMeshProUGUI[] textItems;  // อาร์เรย์ของ TextMeshPro ที่ใช้ในเมนู
    public RectTransform Hilight;  // ตัวแสดงข้อความที่เลือก
    private Vector3 oldPagePosition;
    private readonly int index = 2;
    private string[][] saveData;
    // private string[] saveIDs = { "Save 1", "Save 2", "Save 3", "Save 4" }; 
    // private string[] saveDetails = { "Forest Of Navana", "Maneval Cliff", "Danareval Cave", "Empty" };
    // private string[] saveDates = { "(15:29:23) 2025-02-18", "(02:57:14) 2025-02-17", "(22:01:45) 2025-02-16", "" };
    public static bool isLoad;
    public static bool canDeleteSave;
    private GameObject campfire;

    [Obsolete]
    void Start()
    {
        mainUICode = FindObjectOfType<MainUICode>();  // เพิ่ม MainUICode
        selectSaveCode = FindObjectOfType<SelectSaveCode>();
        selectSaveSlot = FindObjectOfType<SelectSaveSlot>();
        textItems = savePage.GetComponentsInChildren<TextMeshProUGUI>();

        oldPagePosition = savePage.transform.position;
        saveData = selectSaveCode.getSave();
        UpdateSavePage();

        PageContainer pageContainer = textItems[4].AddComponent<PageContainer>();
        pageContainer.Initialize(0, this); // ส่ง index และ reference ไปยัง SelectSaveCode
        pageContainer = textItems[5].AddComponent<PageContainer>();
        pageContainer.Initialize(1, this); // ส่ง index และ reference ไปยัง SelectSaveCode

        UpdateHighlightBox();
        FindActiveRestUI();
    }

    [Obsolete]
    private void FindActiveRestUI()
    {
        // Log all objects with Player tag
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"Found {players.Length} objects with Player tag:");
        foreach (GameObject obj in players)
        {
            // find player object
            Debug.Log($"- Name: {obj.name}, Active: {obj.activeInHierarchy}, Position: {obj.transform.position}");
            if (obj.name.StartsWith("CampFire"))  // More flexible name checking
            {
                campfire = obj;
                Debug.Log($"Found player object: {obj.name}");
            }
        }
    }

    private void UpdateSavePage()
    {
        SaveData data = SaveManager.Instance.LoadGame(ISelectSave.selectedData);

        textItems[0].text = data.saveID;
        textItems[2].text = data.sceneName;
        if (data.isEmpty) { 
            textItems[1].text = "";
            textItems[3].text = "";
        }
        else {
            textItems[1].text = "Playtime " + data.playTime;
            textItems[3].text = data.lastPlayedDate;
        }

        if (data.isEmpty && isLoad) { textItems[4].text = "NewGame"; }
        else if (!isLoad) { textItems[4].text = "Save"; } // isSave
        else { textItems[4].text = "LoadGame"; }
        if (canDeleteSave) { textItems[5].text = "Delete Save"; }
        else { textItems[5].text = "Cancel"; }
        if (data.isEmpty) { textItems[5].text = "Cancel"; } // finlally if empty
    }

    [Obsolete]
    void Update()
    {
        if ((SelectSaveCode.getIsSelectSavePage() && base.CheckTransitionCooldown())
         || (SelectSaveSlot.getIsSelectSaveSlot() && base.CheckTransitionCooldown()))
        {
            EnterTransition();
            UpdateSavePage();
            
            // เพิ่มการรับ Input แบบเดียวกับ SelectSaveCode
            if (Input.GetKeyDown(KeyCode.Z)) 
            {
                FindActiveRestUI();
                SelectAction(currentIndex);
                base.SetTransitionCooldown();
            }
            base.ListenerByLeftRight(2); // 2 คือจำนวนตัวเลือกในเมนู
            UpdateHighlightBox();
        } 
        else 
        { 
            ExitTransition(); 
            currentIndex = 0;
        }
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

    [Obsolete]
    public void SelectAction(int index)
    {
        SaveData data = SaveManager.Instance.LoadGame(ISelectSave.selectedData);
        if (index == 0) // ถ้าเลือกเล่นเซฟ
        {
            if (data.isEmpty && isLoad) // new game
            { 
                SaveManager.Instance.NewGame(ISelectSave.selectedData);
                GameManager.Instance.LoadMap("Toturial");
            } 
            else if (isLoad) // load
            { 
                SaveManager.Instance.LoadGame(ISelectSave.selectedData);
                GameManager.Instance.tempPlayerPosition = data.playerPosition;
                GameManager.Instance.LoadMap(data.sceneName);
            }
            else // save
            { 
                if (campfire != null)
                {
                    Vector3 vector3 = campfire.transform.position;
                    vector3.y += 2f;
                    SaveManager.Instance.SaveGame(ISelectSave.selectedData, vector3);
                    Debug.Log($"Saving game at position: {campfire}");
                }
                else
                {
                    Debug.LogError("No active campfirePostition found for saving position!");
                }
            }
        } 
        else if (index == 1) // ถ้าเลือกลบเซฟ
        {
            if (canDeleteSave) { SaveManager.Instance.DeleteSave(ISelectSave.selectedData); }

            // if (SelectSaveCode.getIsSelectSavePage()) { SelectSaveCode.setIsSelectSavePage(false); }
            // if (SelectSaveSlot.getIsSelectSaveSlot()) { SelectSaveSlot.setIsSelectSaveSlot(false); }

            base.SetTransitionCooldown();
        }
        // update save data
        if (SelectSaveCode.getIsSelectSavePage()) { 
            selectSaveCode.LoadSaveData();
            selectSaveCode.CreateSaveContainers();
        }
        if (SelectSaveSlot.getIsSelectSaveSlot()) { 
            selectSaveSlot.LoadSaveData();
        }

        if (SelectSaveCode.getIsSelectSavePage()) { SelectSaveCode.setIsSelectSavePage(false); }
        if (SelectSaveSlot.getIsSelectSaveSlot()) { SelectSaveSlot.setIsSelectSaveSlot(false); }
    }

    public void OnPointerEnter(int index) // ฟังก์ชั่นที่จะถูกเรียกเมื่อเมาส์ชี้ไปที่ตัวเลือก
    {
        currentIndex = index;
        UpdateHighlightBox();
    }
    public void OnPointerClick(int index) // ฟังก์ชั่นที่จะถูกเรียกเมื่อเมาส์ชี้ไปที่ตัวเลือก
    {
        currentIndex = index;
        SelectAction(currentIndex);
    }
}

class PageContainer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private SavePage savePage;
    private int index;

    public void Initialize(int index, SavePage savePage)
    {
        this.index = index;
        this.savePage = savePage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        savePage.OnPointerEnter(index);
    }

    [Obsolete]
    public void OnPointerClick(PointerEventData eventData)
    {
        savePage.SelectAction(index);
    }
}