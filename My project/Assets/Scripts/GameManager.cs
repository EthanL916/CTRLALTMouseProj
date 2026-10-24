using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource scrubAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip startMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip boxFlipSound;
    [SerializeField] private AudioClip wrongToolSound;
    [SerializeField] private AudioClip cleanedSound;

    [Header("UI Panels")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject progressBarUI;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text finalTimeText;

    [Header("Gameplay & Tools")]
    [SerializeField] private CleaningCursor toolFollower;
    [SerializeField] private List<CleaningTool> tools = new List<CleaningTool>();
    [SerializeField] private LayerMask dirtLayer;
    [SerializeField] private float cleaningRadius = 0.5f;

    // Timers & States
    private float elapsedTime = 0f;
    private float stopTimer = 0f;
    private float wrongToolTimer = 0f;
    private int currentCleaningTool = 0;
    
    private bool isGameActive = false;
    private bool isCleaning = false;
    private bool isInstructionsOpen = false;
    private bool isTimerRunning = true;

    private Vector3 lastToolPosition;

    private void Awake()
    {
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
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }

        if (startMenuPanel == null)
        {
            StartGame();
            return;
        }

        isGameActive = false;
        Cursor.visible = true;

        startMenuPanel.SetActive(true);
        if (progressBarUI != null) progressBarUI.SetActive(false);
        if (toolFollower != null) toolFollower.enabled = false;

        PlayMusic(startMenuMusic);
    }

    private void Update()
    {
        if (isGameActive == false) return;

        if (wrongToolTimer > 0f)
        {
            wrongToolTimer -= Time.deltaTime;
        }

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

        PlayMusic(gameplayMusic);
    }

    private void CleaningToolSwitching()
    {
        if (isCleaning || Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0f)
        {
            currentCleaningTool++;
            if (currentCleaningTool >= tools.Count)
            {
                currentCleaningTool = 0;
            }
            ApplyCleaningToolSprite();
        }
        else if (scroll < 0f)
        {
            currentCleaningTool--;
            if (currentCleaningTool < 0)
            {
                currentCleaningTool = tools.Count - 1;
            }
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

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isCleaning = true;
            lastToolPosition = toolFollower.transform.position;
        }

        if (Mouse.current.leftButton.isPressed && isCleaning)
        {
            Vector3 currentPos = toolFollower.transform.position;
            float distanceMoved = Vector3.Distance(currentPos, lastToolPosition);

            if (distanceMoved >= 0.1f)
            {
                PerformCleaning();

                if (!isGameActive)
                {
                    isCleaning = false;
                    StopScrubSound();
                    return;
                }

                lastToolPosition = currentPos;
                toolFollower.ToggleBubbles(true);
                stopTimer = 0.15f;
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
            toolFollower.ToggleBubbles(false);
            stopTimer = 0f;
            StopScrubSound();
        }

        if (isCleaning && stopTimer > 0f)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0f)
            {
                StopScrubSound();
            }
        }
    }

    private void PerformCleaning()
    {
        if (toolFollower == null || tools.Count == 0) return;

        ToolType activeTool = tools[currentCleaningTool].type;
        ToolType? requiredTool = GetRequiredToolForCurrentDirt(); 

        // Wrong Tool
        if (requiredTool != null && activeTool != requiredTool)
        {
            PlayWrongToolSound();
            toolFollower.ToggleBubbles(false);
            StopScrubSound();
            return; 
        }

        // Correct Tool
        Vector2 toolPos = toolFollower.transform.position;
        Collider2D[] hitDirt = Physics2D.OverlapCircleAll(toolPos, cleaningRadius, dirtLayer);

        foreach (Collider2D dirt in hitDirt)
        {
            DirtTile dirtTile = dirt.GetComponent<DirtTile>();
            if (dirtTile != null)
            {
                dirtTile.CleanTile(activeTool);
            }
        }
    }

    private ToolType? GetRequiredToolForCurrentDirt()
    {
        Vector2 toolPos = toolFollower.transform.position;
        Collider2D[] hitDirt = Physics2D.OverlapCircleAll(toolPos, cleaningRadius, dirtLayer);

        foreach (Collider2D dirt in hitDirt)
        {
            DirtTile dirtTile = dirt.GetComponent<DirtTile>();
            if (dirtTile != null)
            {
                return dirtTile.requiredTool;
            }
        }

        return null;
    }

    // Info Controls
    public void ToggleInstructions()
    {
        if (isInstructionsOpen)
        {
            CloseInstructions();
        }
        else
        {
            OpenInstructions();
        }
    }

    public void OpenInstructions()
    {
        if (instructionsPanel == null)
        {
            instructionsPanel = GameObject.Find("InstructionsPanel");
            if (instructionsPanel == null) return;
        }

        isInstructionsOpen = true;
        instructionsPanel.SetActive(true);
    }

    public void CloseInstructions()
    {
        if (instructionsPanel == null) return;

        isInstructionsOpen = false;
        instructionsPanel.SetActive(false);
    }

    // Audio Players
    public void PlayMusic(AudioClip newClip)
    {
        if (bgmAudioSource == null || newClip == null) return;
        if (bgmAudioSource.clip == newClip && bgmAudioSource.isPlaying) return;

        bgmAudioSource.clip = newClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    private void PlayScrubSound()
    {
        if (scrubAudioSource == null || tools.Count == 0) return;

        AudioClip currentClip = tools[currentCleaningTool].cleaningSound;
        if (currentClip == null) return;

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

    public void PlayCleanedSound()
    {
        if (sfxAudioSource != null && cleanedSound != null)
        {
            sfxAudioSource.PlayOneShot(cleanedSound);
        }
    }
    public void PlayWrongToolSound()
    {
        if (sfxAudioSource != null && wrongToolSound != null && wrongToolTimer <= 0f)
        {
            sfxAudioSource.PlayOneShot(wrongToolSound);
            wrongToolTimer = 0.4f;
        }
    }

    public void PlayFlipSound()
    {
        if (audioSource != null && boxFlipSound != null)
        {
            audioSource.PlayOneShot(boxFlipSound);
        }
    }

    private void ApplyCleaningToolSprite()
    {
        if (tools.Count == 0 || toolFollower == null) return;
        CleaningTool active = tools[currentCleaningTool];
        toolFollower.UpdateCleaningSprite(active.toolSprite, active.activeSprite, active.type);
    }

    // Timer and Win Screen
    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = (int)(elapsedTime / 60f);
        int seconds = (int)(elapsedTime % 60f);

        // Standard format: "00:00"
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    public void TriggerWinScreen()
    {
        isGameActive = false;
        isCleaning = false;
        isTimerRunning = false;

        StopScrubSound();

        if (toolFollower != null)
        {
            toolFollower.ToggleBubbles(false);
            toolFollower.enabled = false;
        }

        Cursor.visible = true;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        PlayMusic(victoryMusic);

        if (finalTimeText != null)
        {
            int minutes = (int)(elapsedTime / 60f);
            int seconds = (int)(elapsedTime % 60f);
            finalTimeText.text = "Clear Time: " + string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }
    }

    // Levels 
    public void RestartGame()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    public void LoadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            SceneManager.LoadScene(0); // Return to Main Menu
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
