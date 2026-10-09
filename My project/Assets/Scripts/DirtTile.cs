using UnityEngine;

public class DirtTile : MonoBehaviour
{
    [Header("Tile Health/Cleanliness")]
    [SerializeField] private float maxDirtiness = 100f;
    [SerializeField] private float currentDirtiness = 100f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject cleanPopupPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Tool Compatibility")]
    [Tooltip("Which cleaning tool is effective against this specific dirt spot?")]
    [SerializeField] private ToolType requiredTool = ToolType.Sponge;

    private bool isFullyCleaned = false;

    public float MaxDirtiness => maxDirtiness;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        currentDirtiness = maxDirtiness;
    }

    public void CleanTile(ToolType usedTool)
    {
        // Guard against wrong tool or already cleaned tile
        if (isFullyCleaned || usedTool != requiredTool || currentDirtiness <= 0f) return;

        float previousDirtiness = currentDirtiness;

        currentDirtiness -= Time.deltaTime * 50f;
        currentDirtiness = Mathf.Clamp(currentDirtiness, 0f, maxDirtiness);

        float cleanedAmount = previousDirtiness - currentDirtiness;

        // Send cleaned amount to DirtTracker
        DirtTracker.ReportCleaning(cleanedAmount);

        // Fade out dirt opacity as it gets cleaned
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = currentDirtiness / maxDirtiness;
            spriteRenderer.color = color;
        }

        // Handle tile completion
        if (currentDirtiness <= 0f)
        {
            isFullyCleaned = true;

            // Spawn the floating text/icon popup at the tile's location
            if (cleanPopupPrefab != null)
            {
                Instantiate(cleanPopupPrefab, transform.position, Quaternion.identity);
            }

            gameObject.SetActive(false);
        }
    }
}
