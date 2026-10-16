using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Static Singleton instance so DirtTracker can communicate with GameManager
    public static GameManager Instance { get; private set; }

    [Header("Background Music Settings")]
    [SerializeField] private AudioSource bgmAudioSource; // Dedicated BGM AudioSource
    [SerializeField] private AudioClip startMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip victoryMusic;


    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource scrubAudioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private float soundStopDelay = 0.15f; // Prevents rapid start/stop crackling
    private float stopTimer = 0f;
    [SerializeField] private AudioClip boxFlipSound;


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

    [Header("Timer Settings")]
    [SerializeField] private TMP_Text timerText;
    private float elapsedTime = 0f;
    private bool isTimerRunning = true;

    [Header("Win Screen UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text finalTimeText;

    private int currentCleaningTool = 0;
    private bool isCleaning = false;
    private bool isGameActive = false;
    private Vector3 lastToolPosition;

    private void Awake()
    {
        // Setup Singleton reference
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // If no start menu exists in this scene (e.g. Level 2), start gameplay immediately
        if (startMenuPanel == null)
        {
            StartGame();
            return;
        }

        isGameActive = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        if (progressBarUI != null) progressBarUI.SetActive(false);
        if (toolFollower != null) toolFollower.enabled = false;

        PlayMusic(startMenuMusic);

        if (firstSelectedButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
        
    }

    private void Update()
    {
        if (!isGameActive) return;

        CleaningToolSwitching();
        CleaningInput();

        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
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
        PlayMusic(gameplayMusic);
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (bgmAudioSource == null || newClip == null) return;

        // Don't restart if the exact same music is already playing
        if (bgmAudioSource.clip == newClip && bgmAudioSource.isPlaying) return;

        bgmAudioSource.clip = newClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
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
        if (!isGameActive || toolFollower == null || Mouse.current == null)
        {
            if (isCleaning)
            {
                isCleaning = false;
                StopScrubSound();
            }
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isCleaning = true;
            lastToolPosition = toolFollower.transform.position;
        }

        if (Mouse.current.leftButton.isPressed && isCleaning)
        {
            Vector3 currentToolPosition = toolFollower.transform.position;
            float distanceMoved = Vector3.Distance(currentToolPosition, lastToolPosition);

            if (distanceMoved >= minMovementThreshold)
            {
                PerformCleaning();

                // Exit immediately if PerformCleaning() triggered TriggerWinScreen()
                if (!isGameActive)
                {
                    isCleaning = false;
                    StopScrubSound();
                    return;
                }

                lastToolPosition = currentToolPosition;
                toolFollower.ToggleBubbles(true);

                stopTimer = soundStopDelay;
                PlayScrubSound();
            }
            else 
            {
                toolFollower.ToggleBubbles(false);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isCleaning = false;
            if (toolFollower != null) toolFollower.ToggleBubbles(false);
            stopTimer = 0f;
            StopScrubSound();
        }

        // Count down timer and pause audio when movement stops
        if (isCleaning && stopTimer > 0f)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0f)
            {
                StopScrubSound();
            }
        }
    }

    private void PlayScrubSound()
    {
        if (scrubAudioSource == null || tools == null || tools.Count == 0) return;

    AudioClip currentClip = tools[currentCleaningTool].cleaningSound;
        if (currentClip == null) return;

    // Swap clip if tool changed
    if (scrubAudioSource.clip != currentClip)
        {
            scrubAudioSource.clip = currentClip;
            scrubAudioSource.loop = true;
        }

        if (!scrubAudioSource.isPlaying)
        {
            scrubAudioSource.Play();
        }
    }

    private void StopScrubSound()
    {
        if (scrubAudioSource != null && scrubAudioSource.isPlaying)
        {
            scrubAudioSource.Pause();
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

    public void PlayFlipSound()
    {
        if (audioSource != null && boxFlipSound != null)
        {
            audioSource.loop = false; // Prevents the flip clip from looping endlessly
            audioSource.PlayOneShot(boxFlipSound);
        }
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

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        // Formats as "01:23.45"
        timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public float GetFinalTime()
    {
        return elapsedTime;
    }

    public void TriggerWinScreen()
    {
        // 1. Immediately halt game state, scrubbing flags, and sound timer
        isGameActive = false;
        isCleaning = false;
        stopTimer = 0f;

        // 2. Stop the active timer
        StopTimer();

        // 3. Kill scrubbing audio loop immediately
        if (scrubAudioSource != null)
        {
            scrubAudioSource.Stop();
        }

        // 4. Disable bubbles and tool follower script so it stops tracking mouse on UI
        if (toolFollower != null)
        {
            toolFollower.ToggleBubbles(false);
            toolFollower.enabled = false;
        }

        // 5. Unhide mouse cursor so player can click win buttons
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 6. Display the Win Screen panel
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        else if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
        }

        // 7. Play victory music
        if (victoryMusic != null)
        {
            PlayMusic(victoryMusic);
        }

        // 8. Format final time string
        if (finalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

            string formattedTime = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
            finalTimeText.text = "Clear Time: " + formattedTime;
        }
    }
    public void LoadNextLevel()
    {
        StartCoroutine(PlaySoundAndNextLevel());
    }
    private IEnumerator PlaySoundAndNextLevel()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
            yield return new WaitForSecondsRealtime(buttonClickSound.length);
        }

    int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

    // Check if there is another level in Build Settings
    if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // If it was the last level, return to the start/main menu (Scene 0)
            SceneManager.LoadScene(0);
        }
    }
    
}
