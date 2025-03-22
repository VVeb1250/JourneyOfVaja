using System;
using UnityEngine; 

[Serializable]
public class SaveData
{
    public string saveID;
    public string sceneName;
    public string playTime; // formated playtime
    public float playTimeNum;
    public string lastPlayedDate;
    public bool isEmpty;
    public float previoustime;
    public float currentTime;
    public Vector3 playerPosition;

    public SaveData()
    {
        isEmpty = true;
        saveID = "";
        sceneName = "Empty";
        playTime = "";
        lastPlayedDate = "";
        playerPosition = Vector3.zero;
    }
    public SaveData(string saveID, string sceneName)
    {
        isEmpty = false;
        this.saveID = saveID;
        this.sceneName = sceneName;
        lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd");
    }
    public SaveData(string saveID, string sceneName, float playTimeNum, Vector3 playerPosition)
    {
        isEmpty = false;
        this.saveID = saveID;
        this.sceneName = sceneName;
        this.playTimeNum = playTimeNum;
        lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd");
        this.playerPosition = playerPosition;
    }

    public static string GetPlayTime(float playTimeNum)
    {
        TimeSpan time = TimeSpan.FromSeconds(playTimeNum);
        
        if (time.Days > 0)
        {
            // Format with days: "DD:HH:mm:ss"
            return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", 
                time.Days,
                time.Hours,
                time.Minutes,
                time.Seconds);
        }
        else
        {
            // Format without days: "HH:mm:ss"
            return string.Format("{0:D2}:{1:D2}:{2:D2}", 
                (int)time.TotalHours,
                time.Minutes,
                time.Seconds);
        }
    }
}