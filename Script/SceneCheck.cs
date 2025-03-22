using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCheck : MonoBehaviour
{
    public string sceneName;
    public GameObject player;
    public Vector2 playerPosition;

    [System.Obsolete]
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag("Player"))
        {
            GameManager.Instance.tempPlayerPosition = playerPosition;
            // change scene
            GameManager.Instance.LoadMap(sceneName);
        }
    }

    [System.Obsolete]
    private void OnEnable()
    {
        if (GameManager.Instance.tempPlayerPosition == Vector2.positiveInfinity) // default value condition
        {
            GameManager.Instance.tempPlayerPosition = SceneManager.GetActiveScene().GetRootGameObjects()[2].transform.position; // vaja (index 2) position
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    [System.Obsolete]
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [System.Obsolete]
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetupScene());
    }

    [System.Obsolete]
    private IEnumerator SetupScene()
    {
        // Wait for scene to be fully loaded
        yield return new WaitForEndOfFrame();

        // Find player in the new scene
        StartCoroutine(SetupPosition());
    }

    [System.Obsolete]
    private IEnumerator SetupPosition()
    {
        // Wait for scene to fully initialize
        yield return new WaitForSeconds(0.1f);

        // Log all objects with Player tag
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"Found {players.Length} objects with Player tag:");
        foreach (GameObject obj in players)
        {
            // find player object
            Debug.Log($"- Name: {obj.name}, Active: {obj.activeInHierarchy}, Position: {obj.transform.position}");
            if (obj.name.StartsWith("Vaja"))  // More flexible name checking
            {
                player = obj;
                Debug.Log($"Found player object: {player.name}");
            }
        }

        if (player != null)
        {
            // Log detailed player information
            Debug.Log($"Found player object: {player.name} at {player.transform.position}");
            
            if (player.name.StartsWith("Vaja"))  // More flexible name checking
            {
                player.transform.position = GameManager.Instance.tempPlayerPosition;
                Debug.Log($"Positioned player at: {GameManager.Instance.tempPlayerPosition}");
            }
            else
            {
                Debug.LogError($"Player object has incorrect name: '{player.name}'. Expected name starting with 'Vaja'");
            }
        }
        else
        {
            Debug.LogError("Failed to find any GameObject with Player tag");
        }
    }
}