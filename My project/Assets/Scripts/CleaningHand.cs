using UnityEngine;

public class ToolCursorFollower : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure System Cursor is hidden
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Camera.main == null)
        {
            Debug.LogError("[ToolCursorFollower] No camera tagged 'MainCamera' found in scene!");
            return;
        }

        // 1. Get raw screen mouse input
        Vector3 mouseScreenPos = Input.mousePosition;

        // 2. Set depth relative to Camera position (e.g. if Cam is Z=-10, depth is 10)
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        // 3. Convert to world coords
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        worldPos.z = 0f; // Lock Z to 2D plane

        // 4. Force position update
        transform.position = worldPos;

        // Debug output to Console
        Debug.Log($"[ToolCursor] Mouse Pos: {Input.mousePosition} | World Pos: {transform.position}");
    }

    public void UpdateToolSprite(Sprite newToolSprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newToolSprite;
        }
    }
}
