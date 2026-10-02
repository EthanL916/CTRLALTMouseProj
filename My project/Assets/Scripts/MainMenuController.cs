using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject winScreenPanel;
    [SerializeField] private GameObject progressBarUI;

    

    [Header("Game Controllers to Enable/Disable")]
    [SerializeField] private GameController gameController;
    [SerializeField] private CleaningCursor cleaningCursor;

    private void Start()
    {
        // Pause/Disable gameplay controls while on the start menu
        if (gameController != null) gameController.enabled = false;
        if (cleaningCursor != null) cleaningCursor.enabled = false;

        //Computer cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Show start menu
        if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(true);
        }

        // HIDE progress bar during the start menu
        if (progressBarUI != null)
        {
            progressBarUI.SetActive(false);
        }
    }

    public void StartGame()
    {
        // Hide the start screen
        if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(false);
        }

        // SHOW progress bar now that gameplay started
        if (progressBarUI != null)
        {
            progressBarUI.SetActive(true);
        }

        // Enable gameplay controls
        if (gameController != null) gameController.enabled = true;
        if (cleaningCursor != null) cleaningCursor.enabled = true;

        // Hide computer cursor so CleaningCursor handles input
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        // Quit standard built standalone application
        Application.Quit();

        #if UNITY_EDITOR
        // Stop Play Mode if running inside Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void RestartGame()
    {
        // Reloads current scene to reset all dirt tiles and progress
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        // Reloads scene, starting fresh back on the main menu panel
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}