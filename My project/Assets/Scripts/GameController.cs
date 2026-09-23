using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public enum ToolType
{
    Sponge,
    Rag,
    GlassCleaner,
    Duster
}

[System.Serializable]
public class CleaningTool
{
    public string toolName;
    public ToolType type;
    public Sprite toolSprite;
}
public class GameController : MonoBehaviour
{
    [Header("Tool Inventory")]
    [SerializeField] private List<CleaningTool> tools = new List<CleaningTool>();
    private int currentCleaningTool = 0;

    [Header("GameObject Cursor Reference")]
    [SerializeField] private CleaningCursor toolFollower;

    [Header("Cleaning Settings")]
    [SerializeField] private LayerMask dirtLayer;
    [SerializeField] private float cleaningRadius = 0.5f;

    private bool isCleaning = false;

    private void Start()
    {
        Cursor.visible = false;
        if (tools.Count > 0 && toolFollower != null)
        {
            ApplyCleaningToolSprite();
        }
    }

    private void Update()
    {
        CleaningToolSwitching();
        CleaningInput();
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
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isCleaning = true;
        }
        if (Mouse.current.leftButton.isPressed)
        {
            PerformCleaning();
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isCleaning = false;
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
        toolFollower.UpdateCleaningSprite(activeTool.toolSprite);
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }
}
