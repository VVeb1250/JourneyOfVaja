using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Header("File Stroage Config")]
    [SerializeField] private string filename;
    private GameData gameData;
    private List<IDataPersistence> dataPersistencesObjects;
    private FileDataHandler dataHandler;
    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) 
        {
            Debug.LogError("Found more than one data in DataManager in the scene.");
        } 
        Instance = this;
    }

    [System.Obsolete]
    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, filename);
        this.dataPersistencesObjects = FindAllDataPersistencesObjects();
        LoadGame();
    }

    [System.Obsolete]
    private List<IDataPersistence> FindAllDataPersistencesObjects()
    {
        // ค้นหาทุกออบเจกต์ MonoBehaviour
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        List<IDataPersistence> dataPersistencesObjects = new();

        // ตรวจสอบว่าแต่ละออบเจกต์ที่ค้นพบมีการ Implement อินเตอร์เฟซ IDataPersistence หรือไม่
        foreach (MonoBehaviour monoBehaviour in allMonoBehaviours)
        {
            if (monoBehaviour is IDataPersistence dataPersistence)
            {
                dataPersistencesObjects.Add(dataPersistence);
            }
        } return dataPersistencesObjects;
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }
    public void LoadGame()
    {
        // Load any save data
        this.gameData = dataHandler.Load();
        // load file data from file
        if (this.gameData == null)
        {
            Debug.Log("Data was not found.");
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}