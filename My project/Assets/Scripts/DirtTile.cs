using UnityEngine;

public class DirtTile : MonoBehaviour
{
    [Header("Tile Health/Cleanliness")]
    [SerializeField] private float maxDirtiness = 100f;
    [SerializeField] private float currentDirtiness = 100f;

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

    // This method is called when the player uses a cleaning tool on this tile
    public void CleanTile(ToolType usedTool)
    {
        if (usedTool != requiredTool) return;
        
        currentDirtiness -= Time.deltaTime * 50f; // Decreasing dirtiness by a fixed amount
        currentDirtiness = Mathf.Clamp(currentDirtiness, 0f, maxDirtiness);

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = currentDirtiness / maxDirtiness;;
            spriteRenderer.color = color;
        }

        if (currentDirtiness <= 0f)
        {
            gameObject.SetActive(false); // Tile deactivated when cleaned
        }
    }
}
