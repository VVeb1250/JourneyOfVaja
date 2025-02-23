using UnityEngine;

public abstract class MyTransition : MonoBehaviour
{
    protected float TransitionSpeed = 5f;
    protected int currentIndex = 0;  // ตัวแปรที่ใช้เก็บตำแหน่งเมนูที่ถูกเลือก
    // delay
    protected readonly float delayTime = 0.25f;
    protected float totalDelayTime = 0f;
    public bool ListenerByKeys(int itemLength) {
        // เก็บค่าการเคลื่อนไหว
        float verticalInput = Input.GetAxis("Vertical");
        // การควบคุมด้วยลูกศร
        if (verticalInput <= -0.001f) // ชี้ลง
        {
            this.currentIndex = (this.currentIndex + 1) % itemLength;  // เลือกเมนูถัดไป
            Input.ResetInputAxes();
            return true;
        }
        else if (verticalInput >= 0.001f) // ชี้ขึ้น
        {
            this.currentIndex = (this.currentIndex - 1 + itemLength) % itemLength;  // เลือกเมนูก่อนหน้า
            Input.ResetInputAxes();
            return true;
        }
        return false;
    }
    public bool ListenerByLeftRight(int itemLength) {
        // เก็บค่าการเคลื่อนไหว
        float verticalInput = Input.GetAxis("Horizontal");
        // การควบคุมด้วยลูกศร
        if (verticalInput <= -0.001f) // ซ้าย
        {
            this.currentIndex = (this.currentIndex + 1) % itemLength;  // เลือกเมนูถัดไป
            Input.ResetInputAxes();
            return true;
        }
        else if (verticalInput >= 0.001f) // ขวา
        {
            this.currentIndex = (this.currentIndex - 1 + itemLength) % itemLength;  // เลือกเมนูก่อนหน้า
            Input.ResetInputAxes();
            return true;
        }
        return false;
    }
    public bool CheckTransitionCooldown()
    {
        return Time.time > totalDelayTime;
    }
    public void SetTransitionCooldown()
    {
        totalDelayTime = Time.time + delayTime;
        Input.ResetInputAxes();
    }
    protected abstract void EnterTransition();
    protected abstract void ExitTransition();
}
