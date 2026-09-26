using UnityEngine;

public class DirtTile : MonoBehaviour
{
    [Header("Tile Health/Cleanliness")]
    [SerializeField] private float maxDirtiness = 100f;
    [SerializeField] private float currentDirtiness = 100f;

    public float MaxDirtiness => maxDirtiness;

    [Header("Tool Compatibility")]
    [Tooltip("Which cleaning tool is effective against this specific dirt spot?")]
    [SerializeField] private ToolType requiredTool = ToolType.Sponge;
    
    [SerializeField] private SpriteRenderer spriteRenderer;

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
        if (usedTool != requiredTool || currentDirtiness <= 0f) return;

        float previousDirtiness = currentDirtiness;

        currentDirtiness -= Time.deltaTime * 50f;
        currentDirtiness = Mathf.Clamp(currentDirtiness, 0f, maxDirtiness);

        float cleanedAmount = previousDirtiness - currentDirtiness;

        // Send cleaned amount to DirtTracker
        DirtTracker.ReportCleaning(cleanedAmount);

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = currentDirtiness / maxDirtiness;
            spriteRenderer.color = color;
        }

        if (currentDirtiness <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
