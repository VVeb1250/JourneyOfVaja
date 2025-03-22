using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyNagivatorCode : MonoBehaviour
{
    // ดึงข้อมูลจาก MainUICode
    private MainUICode mainUI;
    private SelectSaveCode selectSaveCode;
    private SavePage savePage;  // เพิ่มการอ้างอิง SavePage
    // การอ้างอิงถึง Prefab ที่มี Image และ Text
    public GameObject[] keyPrefab;  // Prefab ที่มี Image และ Text
    public Transform parentTransform; // Parent ของคีย์ที่จะถูก Instantiate
    private bool[] isActivate;

    [System.Obsolete]
    void Start()
    {
        mainUI = FindObjectOfType<MainUICode>();  // เพิ่ม MainUICode
        selectSaveCode = FindObjectOfType<SelectSaveCode>();  // เพิ่ม SelectSaveCode
        savePage = FindObjectOfType<SavePage>();  // เพิ่มการค้นหา SavePage component
        isActivate = new bool[keyPrefab.Length];
    }

    void Update()
    {
        if (SelectSaveCode.getIsSelectSavePage())
        {
            ShowSavePageKey();
        }
        else if (MainUICode.getIsSelectSave())
        {
            ShowSaveKey();
        }
        else if (MainUICode.getIsSelectSetting())
        {
            ShowSettingKey();
        }
        else
        {
            ShowMainKey();
        }
        UpdateKey();
    }

    public void UpdateKey()
    {
        float oldXPosition = 0;
        for (int i = 0; i < keyPrefab.Length; i++)
        {
            if (!isActivate[i]) 
            {
                keyPrefab[i].SetActive(false);
                continue;
            }
            keyPrefab[i].SetActive(true);
            Vector3 newPosition = parentTransform.position; // set origimal position
            newPosition.x += oldXPosition; // move by oldpos
            keyPrefab[i].transform.position = newPosition; // set new position
            // หาขนาดของ UI GameObject
            if (keyPrefab[i].TryGetComponent<RectTransform>(out var rectTransform))
            {
                // sizeDelta จะให้ขนาดของ GameObject ในหน่วยพิกเซล
                Vector2 size = rectTransform.sizeDelta;
                oldXPosition += size.x;
            }
        }
    }
    public void ShowSaveKey()
    {
        isActivate[0] = true;
        isActivate[1] = true;
        isActivate[2] = true;
        isActivate[3] = true;
        isActivate[4] = false;
        isActivate[5] = false;
        foreach (GameObject item in keyPrefab)
        {
            SetKeyColor(item, Color.black);
        }
    }
    public void ShowSavePageKey()
    {
        isActivate[0] = true;
        isActivate[1] = true;
        isActivate[2] = false;
        isActivate[3] = false;
        isActivate[4] = true;
        isActivate[5] = true;
        foreach (GameObject item in keyPrefab)
        {
            SetKeyColor(item, Color.black);
        }
    }
    public void ShowSettingKey()
    {
        isActivate[0] = true;
        isActivate[1] = true;
        isActivate[2] = true;
        isActivate[3] = true;
        isActivate[4] = true;
        isActivate[5] = true;
        foreach (GameObject item in keyPrefab)
        {
            SetKeyColor(item, Color.black);
        }
    }
    public void ShowMainKey() 
    {
        isActivate[0] = true;
        isActivate[1] = false;
        isActivate[2] = true;
        isActivate[3] = true;
        isActivate[4] = false;
        isActivate[5] = false;
        foreach (GameObject item in keyPrefab)
        {
            SetKeyColor(item, Color.white);
        }
    }
    public void SetKeyColor(GameObject gameObject, Color color)
    {
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            image.color = color;
        }
        TextMeshProUGUI[] texts = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            text.color = color;
        }
    }
}