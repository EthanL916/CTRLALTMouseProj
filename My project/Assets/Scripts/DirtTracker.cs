using UnityEngine;
using TMPro; // Required for TextMeshPro UI elements

public class DirtTracker : MonoBehaviour
{
    public static DirtTracker Instance { get; private set; }

    [Header("UI & Progress")]
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private TextMeshProUGUI winTimeText; // Text element on win screen

    [Header("Victory Screen & Managers")]
    [SerializeField] private GameObject winScreenPanel;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CleaningCursor cleaningCursor;

    private float totalLevelDirt = 0f;
    private float currentProgressFraction = 0f; // Tracks total fill from 0.0 to 1.0
    private bool hasWon = false;
    private float elapsedTime = 0f; // Tracks total play time

    private void Awake()
    {
        // Force Instance to point to THIS new active object when the scene reloads
        Instance = this;

        if (progressBar == null)
        {
            progressBar = Object.FindFirstObjectByType<ProgressBar>();
        }

        if (gameManager == null)
        {
            gameManager = Object.FindFirstObjectByType<GameManager>();
        }

        if (cleaningCursor == null)
        {
            cleaningCursor = Object.FindFirstObjectByType<CleaningCursor>();
        }
    }

    private void Start()
    {
        // Reset runtime tracking variables on scene load
        hasWon = false;
        currentProgressFraction = 0f;
        elapsedTime = 0f;

        CalculateTotalLevelDirt();

        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Accumulate elapsed time until the player triggers victory
        if (!hasWon)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    private void CalculateTotalLevelDirt()
    {
        totalLevelDirt = 0f;

        // Finds active DirtTile components in the scene directly
        DirtTile[] allTiles = Object.FindObjectsByType<DirtTile>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (DirtTile tile in allTiles)
        {
            totalLevelDirt += tile.MaxDirtiness;
        }
    }

    public static void ReportCleaning(float cleanedAmount)
    {
        if (Instance != null && Instance.totalLevelDirt > 0f)
        {
            Instance.ProcessCleaningProgress(cleanedAmount);
        }
    }

    private void ProcessCleaningProgress(float cleanedAmount)
    {
        if (hasWon) return;

        // 1. Calculate progress step (0.0 to 1.0)
        float progressStep = cleanedAmount / totalLevelDirt;
        currentProgressFraction += progressStep;

        // 2. Update existing ProgressBar
        if (progressBar != null)
        {
            progressBar.IncreaseProgress(progressStep);
        }

        // 3. Trigger Victory when progress reaches 100% (1.0)
        if (currentProgressFraction >= 0.999f)
        {
            TriggerWin();
        }
    }

    private void TriggerWin()
    {
        hasWon = true;

        // Format elapsed seconds into MM:SS format
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        if (winTimeText != null)
        {
            winTimeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }

        // Lock gameplay controls
        if (gameManager != null) gameManager.enabled = false;
        if (cleaningCursor != null) cleaningCursor.enabled = false;

        // Make standard mouse cursor visible for victory menu interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Display victory overlay
        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
        }
    }
}