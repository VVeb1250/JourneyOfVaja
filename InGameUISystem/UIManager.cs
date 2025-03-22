using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject restUI;
    public GameObject pauseUI;
    private CameraController cameraController;
    private PlayerController playerController;
    private float zoomSize = 7.5f; // How close to zoom in
    private float zoomDuration = 0.5f; // How long the zoom takes
    public bool isResting = false;
    public bool isPausing = false;
    private bool playerInRange = false;

    [Obsolete]
    void Start()
    {
        restUI.SetActive(false);
        pauseUI.SetActive(false);
        cameraController = FindObjectOfType<CameraController>();
        playerController = FindObjectOfType<PlayerController>();
        // Find CameraController on the CinemachineVirtualCamera GameObject
    }


    void Update()
    {
        if (!isResting && !isPausing && Input.GetKeyDown(KeyCode.Escape))
        {
            EnterPauseUI();
            Input.ResetInputAxes();
        }
        if ( isPausing && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X)) )
        {
            ExitPauseUI();
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.V))
        {
            EnterRestUI();
            cameraController.ZoomToSize(zoomSize, zoomDuration);
            Debug.Log("Enter Rest");
        }

        if (isResting && Input.GetKeyDown(KeyCode.X))
        {
            ExitRestUI();
            Debug.Log("Exit Rest");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    private void EnterRestUI()
    {
        isResting = true;
        playerController.setHealth(100);
        playerController.setNumPotion(3);
        restUI.SetActive(true);
    }

    public IEnumerator ExitRestUI()
    {
        yield return new WaitForSeconds(0.25f);
        isResting = false;
        restUI.SetActive(false);
    }
    private void EnterPauseUI()
    {
        isPausing = true;
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
    }
    public void ExitPauseUI()
    {
        isPausing = false;
        Time.timeScale = 1;
        pauseUI.SetActive(false);
        Input.ResetInputAxes();
    }
}