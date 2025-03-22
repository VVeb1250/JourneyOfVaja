using UnityEngine;
using System.IO;
using System;

public class SaveManager : MonoBehaviour
{
    // private static string SavePath => Application.persistentDataPath + "Assets/SaveManager/SaveData/";
    private static string SavePath => Application.persistentDataPath + "/SaveData/";
    private static SaveManager instance;
    private static int oldSaveSelect;

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new("SaveManager");
                instance = go.AddComponent<SaveManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    void Awake()
    {
        if (File.Exists(SavePath) == false)
        {
            Directory.CreateDirectory(SavePath);
            InitializeEmptySaves();
        }
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEmptySaves()
    {
        for (int i = 0; i < 4; i++)
        {
            string path = SavePath + $"save_{i + 1}.json";
            if (!File.Exists(path))
            {
                SaveData emptyData = new();
                string json = JsonUtility.ToJson(emptyData);
                File.WriteAllText(path, json);
            }
        }
    }

    public void NewGame(int slot)
    {
        SaveData save = new($"Save {slot + 1}", "Toturial")
        {
            previoustime = Time.time
        };

        // string json = JsonUtility.ToJson(save);
        // File.WriteAllText(SavePath + $"save_{slot + 1}.json", json);
    }

    public void SaveGame(int slot, Vector3 playerPosition)
    {
        SaveData oldData = Instance.LoadGame(oldSaveSelect);
        SaveData save = new($"Save {slot + 1}", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
        oldData.playTimeNum, playerPosition)
        {
            currentTime = Time.time,
            lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd"),
        };
        save.playTimeNum += save.currentTime - save.previoustime;
        save.playTime = SaveData.GetPlayTime(save.playTimeNum);

        save.previoustime = Time.time;

        string json = JsonUtility.ToJson(save);
        File.WriteAllText(SavePath + $"save_{slot + 1}.json", json);
    }

    public SaveData LoadGame(int slot)
    {
        string path = SavePath + $"save_{slot + 1}.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData loadTemp = JsonUtility.FromJson<SaveData>(json);
            
            loadTemp.previoustime = Time.time;
            return loadTemp;
        }
        oldSaveSelect = slot;
        return new SaveData();
    }

    public void DeleteSave(int slot)
    {
        SaveData emptyData = new();
        string json = JsonUtility.ToJson(emptyData);
        File.WriteAllText(SavePath + $"save_{slot + 1}.json", json);
    }
}