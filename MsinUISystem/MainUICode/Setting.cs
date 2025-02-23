using TMPro;
using UnityEngine;

public class Setting : MyTransition
{
    MainUICode mainUI;
    // header
    public TextMeshProUGUI Header;
    public GameObject settingMember;
    private Vector3 oldHeaderPosition;
    private Vector3 oldSettingMember;
    // transition
    private float selectSaveTransitionTime = 5;

    [System.Obsolete]
    void Start()
    {
        mainUI = FindObjectOfType<MainUICode>();  // เพิ่ม MainUICode
        oldHeaderPosition = Header.transform.position;
        oldSettingMember = settingMember.transform.position;
    }
    void Update()
    {
        if (mainUI.getIsSelectSetting())
        {
            EnterTransition();
        } else { ExitTransition(); }
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