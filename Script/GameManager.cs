using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
//using UnityEditor.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public string sceneName;
    public Vector2 tempPlayerPosition = Vector2.positiveInfinity; // keep track of player position

    [System.Obsolete]
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
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

    public void LoadMap(string sceneName)
    {
        StartCoroutine(LoadMapCoroutine(sceneName));
    }

    private IEnumerator LoadMapCoroutine(string sceneName)
    {
        // Add any transition effects here
        yield return new WaitForSeconds(0.5f);
        
        // Load scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        } 
    }
}
