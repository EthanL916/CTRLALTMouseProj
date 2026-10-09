using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    [Header("UI Panels & Navigation")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject winScreenPanel;
    [SerializeField] private GameObject progressBarUI;
    [SerializeField] private GameObject firstSelectedButton;

    [Header("Tool Inventory & Gameplay")]
    [SerializeField] private CleaningCursor toolFollower;
    [SerializeField] private List<CleaningTool> tools = new List<CleaningTool>();
    [SerializeField] private LayerMask dirtLayer;
    [SerializeField] private float cleaningRadius = 0.5f;
    [SerializeField] private float minMovementThreshold = 0.1f; // Minimum distance the tool must move to count as cleaning

    private int currentCleaningTool = 0;
    private bool isCleaning = false;
    private bool isGameActive = false;
    private Vector3 lastToolPosition;

    private void Start()
    {
        // Disable gameplay controls until Start Game is clicked
        isGameActive = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        if (progressBarUI != null) progressBarUI.SetActive(false);
        if (toolFollower != null) toolFollower.enabled = false;

        if (firstSelectedButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    private void Update()
    {
        if (!isGameActive) return; // Freeze gameplay inputs while menu is active

        CleaningToolSwitching();
        CleaningInput();
    }

    public void StartGame()
    {
        isGameActive = true;
        
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        if (progressBarUI != null) progressBarUI.SetActive(true);
        if (toolFollower != null) toolFollower.enabled = true;

        Cursor.visible = false;

        if (tools.Count > 0 && toolFollower != null)
        {
            ApplyCleaningToolSprite();
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void CleaningToolSwitching()
    {
        if (isCleaning || Mouse.current == null) return;

        float scrollDelta = Mouse.current.scroll.ReadValue().y;
        if (scrollDelta > 0f)
        {
            currentCleaningTool = (currentCleaningTool + 1) % tools.Count;
            ApplyCleaningToolSprite();
        }
        else if (scrollDelta < 0f)
        {
            currentCleaningTool = (currentCleaningTool + tools.Count) % tools.Count;
            ApplyCleaningToolSprite();
        }
    }

    private void CleaningInput()
    {
        if (toolFollower == null || Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isCleaning = true;
            // Record initial position when starting a scrub
            lastToolPosition = toolFollower.transform.position;
        }

        if (Mouse.current.leftButton.isPressed && isCleaning)
        {
            Vector3 currentToolPosition = toolFollower.transform.position;
            float distanceMoved = Vector3.Distance(currentToolPosition, lastToolPosition);

            // Only clean if the tool is actively moving/scrubbing across the surface
            if (distanceMoved >= minMovementThreshold)
            {
                PerformCleaning();
                // Reset tracking position to current spot after scrubbing
                lastToolPosition = currentToolPosition;
                toolFollower.ToggleBubbles(true); // Emit bubbles while moving
            }
            else 
            {
                toolFollower.ToggleBubbles(false); // Stop bubbles if not moving
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isCleaning = false;
            toolFollower.ToggleBubbles(false); // Stop emitting on release
        }
    }

    private void PerformCleaning()
    {
        if (toolFollower == null) return;

        Vector2 toolWorldPosition = toolFollower.transform.position;
        Collider2D[] hitDirt = Physics2D.OverlapCircleAll(toolWorldPosition, cleaningRadius, dirtLayer);

        foreach (Collider2D dirt in hitDirt)
        {
            DirtTile dirtTile = dirt.GetComponent<DirtTile>();
            if (dirtTile != null)
            {
                dirtTile.CleanTile(tools[currentCleaningTool].type);
            }
        }
    }

    private void ApplyCleaningToolSprite()
    {
        CleaningTool activeTool = tools[currentCleaningTool];
        toolFollower.UpdateCleaningSprite(activeTool.toolSprite, activeTool.activeSprite, activeTool.type);
    }

    public void QuitGame()
    {
        StartCoroutine(PlaySoundAndQuit());
    }

    public void RestartGame()
    {
        StartCoroutine(PlaySoundAndRestart());
    }

    private IEnumerator PlaySoundAndQuit()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
            yield return new WaitForSecondsRealtime(buttonClickSound.length);
        }

        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private IEnumerator PlaySoundAndRestart()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
            yield return new WaitForSecondsRealtime(buttonClickSound.length);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
