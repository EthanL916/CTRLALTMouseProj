using UnityEngine;

public class DirtTracker : MonoBehaviour
{
    public static DirtTracker Instance { get; private set; }

    [Header("UI & Progress")]
    [SerializeField] private ProgressBar progressBar;

    [Header("Victory Screen References")]
    [SerializeField] private GameObject winScreenPanel;
    [SerializeField] private GameController gameController;
    [SerializeField] private CleaningCursor cleaningCursor;

    private float totalLevelDirt = 0f;
    private float currentProgressFraction = 0f; // Tracks total fill from 0.0 to 1.0
    private bool hasWon = false;

    private void Awake()
{
    // Fix: Force Instance to point to THIS new active object when the scene reloads
    Instance = this;

    if (progressBar == null)
    {
        progressBar = Object.FindFirstObjectByType<ProgressBar>();
    }

    if (gameController == null)
        gameController = Object.FindFirstObjectByType<GameController>();

    if (cleaningCursor == null)
        cleaningCursor = Object.FindFirstObjectByType<CleaningCursor>();
}

private void Start()
{
    // Reset runtime tracking variables on scene load
    hasWon = false;
    currentProgressFraction = 0f;

    CalculateTotalLevelDirt();

    if (winScreenPanel != null)
    {
        winScreenPanel.SetActive(false);
    }
}

    private void CalculateTotalLevelDirt()
    {
        totalLevelDirt = 0f;

        DirtTile[] allTiles = Resources.FindObjectsOfTypeAll<DirtTile>();

        foreach (DirtTile tile in allTiles)
        {
            if (tile.gameObject.scene.isLoaded)
            {
                totalLevelDirt += tile.MaxDirtiness;
            }
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

        // Lock gameplay controls
        if (gameController != null) gameController.enabled = false;
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