using UnityEngine;

public class DirtTracker : MonoBehaviour
{
    public static DirtTracker Instance { get; private set; }

    [SerializeField] private ProgressBar progressBar;

    private float totalLevelDirt = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (progressBar == null)
        {
            progressBar = Object.FindFirstObjectByType<ProgressBar>();
        }
    }

    private void Start()
    {
        CalculateTotalLevelDirt();
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
            float progressFraction = cleanedAmount / Instance.totalLevelDirt;

            if (Instance.progressBar != null)
            {
                Instance.progressBar.IncreaseProgress(progressFraction);
            }
        }
    }
}