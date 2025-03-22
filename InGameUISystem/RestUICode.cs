using System.Collections;
using TMPro;
using UnityEngine;

public class RestUICode : MyTransition
{
    private GameObject groupMenu;
    private TextMeshProUGUI[] menuItems;
    private Vector3 oldPosition;
    public RectTransform Hilight;

    // ตัวแปรสำหรับเลือกเมนู
    public static bool isSelectSave = false;
    public static bool isSelectSetting = false;

    UIManager uiManager;
    SelectSaveSlot selectSaveSlot;
    // camera transition
    private CameraController cameraController;
    public float zoomSize = 7.5f; // How close to zoom in
    public float zoomDuration = 0.5f; // How long the zoom takes
    private bool isLoad;
    public bool IsLoad() { return this.isLoad; }

    // [System.Obsolete]

    void OnEnable()
    {
        // Initialize components
        groupMenu = this.gameObject;
        menuItems = GetComponentsInChildren<TextMeshProUGUI>();
        
        if (Hilight == null)
        {
            Debug.LogError("Hilight RectTransform not assigned in inspector");
            return;
        }

        // Reset positions if not yet initialized
        if (oldPosition == Vector3.zero)
        {
            oldPosition = groupMenu.transform.position;
            Vector3 position = oldPosition;
            position.x -= 1000;
            groupMenu.transform.position = position;
        }

        // Reset states
        isSelectSave = false;
        isSelectSetting = false;
        SavePage.isLoad = false;
        SelectSaveSlot.setIsSelectSaveSlot(false);
        currentIndex = 0;

        // Setup camera if available
        if (cameraController != null)
        {
            cameraController.ZoomToSize(zoomSize, zoomDuration);
        }

        EnterTransition();
    }

    [System.Obsolete]
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        cameraController = FindObjectOfType<CameraController>();
        // selectSaveSlot = FindObjectOfType<SelectSaveSlot>();
        // cameraController.ZoomToSize(zoomSize, zoomDuration);
        SetupUI();
    }
    private void SetupUI()
    {        
        // Remove duplicate initialization since it's now in OnEnable
        menuItems = groupMenu.GetComponentsInChildren<TextMeshProUGUI>();
        UpdateHighlightBox();
    }

    [System.Obsolete]
    void Update()
    {
        if (base.CheckTransitionCooldown() && !isSelectSave && !isSelectSetting) {
            EnterTransition(); // transition กับ menu
            if (Input.GetKeyDown(KeyCode.Z)) { SelectMenuItem(currentIndex); }
            base.ListenerByKeys(menuItems.Length); // no need cuz always up date hilightbox on EnterTransition()
        } else { 
            ExitTransition();
            CancelSettingCheck();
        }
        if ( !isSelectSave && !isSelectSetting && (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(1)) ) {
            base.SetTransitionCooldown();
            cameraController.ResetZoom(zoomDuration);
            ExitTransition();
            StartCoroutine(uiManager.ExitRestUI());
        }
    }
    private void CancelSettingCheck() {
        if (isSelectSetting && (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(1)) ) { 
            isSelectSetting = false; 
            Input.ResetInputAxes();
        }
    }
    [System.Obsolete]
    public void SelectMenuItem(int index) {
        if (menuItems[index].text == "Depart")
        {
            Debug.Log("เล่นต่อ");
            base.SetTransitionCooldown();

            cameraController.ResetZoom(zoomDuration);
            ExitTransition();
            StartCoroutine(uiManager.ExitRestUI());
        }
        else if (menuItems[index].text == "Save")
        {
            Debug.Log("เซฟเกม");
            isSelectSave = true;
            isLoad = false;
            SavePage.isLoad = false;
            SavePage.canDeleteSave = false;
            base.SetTransitionCooldown();
            
            ExitTransition();
        }
        else if (menuItems[index].text == "Load")
        {
            Debug.Log("โหลดเกม");
            isSelectSave = true;
            isLoad = true;
            SavePage.isLoad = true;
            SavePage.canDeleteSave = false;
            base.SetTransitionCooldown();
            
            ExitTransition();
        }
        else if (menuItems[index].text == "Setting")
        {
            isSelectSetting = true;
            Debug.Log("ไปที่ตั้งค่า");
        }
        else if (menuItems[index].text == "To Main Menu")
        {
            Debug.Log("กลับไปหน้าหลัก");
            GameManager.Instance.LoadMap("MainMenu");
        }
    }
    private void UpdateHighlightBox()
    {
        if (menuItems == null || currentIndex >= menuItems.Length || Hilight == null)
        {
            Debug.LogWarning("Required components not initialized in UpdateHighlightBox");
            return;
        }

        Vector3 tempPos = menuItems[currentIndex].transform.position;
        tempPos.x -= 100;
        Hilight.transform.position = Vector3.Lerp(
            Hilight.transform.position, 
            tempPos, 
            TransitionSpeed * Time.deltaTime * 4);
    }


    protected override void EnterTransition()
    {
        if (groupMenu == null)
        {
            Debug.LogWarning("groupMenu not initialized in EnterTransition");
            return;
        }

        groupMenu.transform.position = Vector3.Lerp(groupMenu.transform.position, oldPosition, TransitionSpeed * Time.deltaTime);
        
        // Only call UpdateHighlightBox if UI is properly initialized
        if (menuItems != null && menuItems.Length > 0 && Hilight != null)
        {
            UpdateHighlightBox();
        }
    }
    protected override void ExitTransition()
    {
        Vector3 tempPosition = oldPosition;
        tempPosition.x -= 1000;
        groupMenu.transform.position = Vector3.Lerp(groupMenu.transform.position, tempPosition, TransitionSpeed * Time.deltaTime);        
        UpdateHighlightBox();

    }

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