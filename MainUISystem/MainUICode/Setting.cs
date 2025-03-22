using TMPro;
using UnityEngine;

public class Setting : MyTransition
{
    // header
    public TextMeshProUGUI Header;
    public GameObject settingMember;
    public GameObject[] settingMembers;
    public GameObject[] soundUI;
    public RectTransform[] bullet;
    public RectTransform Hilight;
    private Vector3 oldHeaderPosition;
    private Vector3 oldSettingMember;
    // transition
    private float selectSaveTransitionTime = 5;

    [System.Obsolete]
    void Start()
    {
        currentIndex = 0;
        oldHeaderPosition = Header.transform.position;
        oldSettingMember = settingMember.transform.position;
    }
    void Update()
    {
        if (MainUICode.getIsSelectSetting() || RestUICode.isSelectSetting)
        {
            EnterTransition();
            base.ListenerByKeys(settingMembers.Length);
            
            // เก็บค่าการเคลื่อนไหว
            float horizontalInput = Input.GetAxis("Horizontal");
            if (horizontalInput <= -0.01f) // ซ้าย
            {
                if (SoundManager.settingSound[currentIndex] > 0) { SoundManager.settingSound[currentIndex] -= 1; }
                Input.ResetInputAxes();
            }
            else if (horizontalInput >= 0.01f) // ขวา
            {
                if (SoundManager.settingSound[currentIndex] < 10) { SoundManager.settingSound[currentIndex] += 1; }
                Input.ResetInputAxes();
            }

        } else { ExitTransition(); }
        UpdateHighlightBox();
        UpdateSoundUI();
    }
    private void UpdateHighlightBox()
    {
        Vector3 newPosition = settingMembers[currentIndex].transform.position;
        // newPosition.x -= 2000;
        // เริ่มต้น Coroutine สำหรับการเคลื่อนที่ของกล่องไฮไลท์
        Hilight.transform.position = Vector3.Lerp(
            Hilight.transform.position, 
            newPosition, 
            TransitionSpeed * Time.deltaTime * 4);
    }
    private void UpdateSoundUI()
    {
        for (int i = 0; i < soundUI.Length; i++) { // เปลี่ยน i > soundUI.Length เป็น i < soundUI.Length
            // ดึง RectTransform ของ soundUI และ bullet
            RectTransform rect = soundUI[i].GetComponent<RectTransform>();
            // คำนวณตำแหน่งใหม่ที่ bulletRect ควรจะอยู่
            Vector2 point = new(
                (rect.rect.width / -2) + (SoundManager.settingSound[i] * (rect.rect.width / 10)), 
                rect.rect.y
            );
            
            // ตั้งค่าตำแหน่งใหม่ของ bulletRect
            // bulletRect.anchoredPosition = new Vector2(point.x, bulletRect.anchoredPosition.y); // ตั้งค่าตำแหน่ง X และ Y
            Vector2 newPosition = rect.transform.position; // ตั้งค่าตำแหน่ง X และ Y
            newPosition.x += point.x;
            // newPosition.y += point.x;
            bullet[i].transform.position = Vector3.Lerp(
                bullet[i].transform.position, 
                newPosition, 
                TransitionSpeed * Time.deltaTime * 4);
        }
    }

    protected override void EnterTransition() {
        Vector3 newHeaderPosition = oldHeaderPosition;
        newHeaderPosition.y -= 500;
        Header.transform.position = Vector3.Lerp(Header.transform.position, newHeaderPosition, selectSaveTransitionTime * Time.deltaTime);
        Vector3 newSettingMember = oldSettingMember;
        newSettingMember.y -= 1100;
        settingMember.transform.position = Vector3.Lerp(settingMember.transform.position, newSettingMember, selectSaveTransitionTime * Time.deltaTime);
    }
    protected override void ExitTransition() {
        Header.transform.position = Vector3.Lerp(Header.transform.position, oldHeaderPosition, selectSaveTransitionTime * Time.deltaTime);
        settingMember.transform.position = Vector3.Lerp(settingMember.transform.position, oldSettingMember, selectSaveTransitionTime * Time.deltaTime);
    }

}