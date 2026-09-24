using UnityEngine;
using UnityEngine.InputSystem;

public class CleaningHand : MonoBehaviour
{
    [Header("Tool Properties")]
    [SerializeField] private ToolType currentToolType = ToolType.Sponge;

    [Header("Cleaning Settings")]
    [SerializeField] private LayerMask dirtLayer = ~0;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        // Clean continuously while holding left mouse button
        if (Mouse.current.leftButton.isPressed)
        {
            PerformCleaning();
        }
    }

    private void PerformCleaning()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCamera.transform.position.z)));
        Vector2 originPoint = new Vector2(worldPoint.x, worldPoint.y);

        // Raycast ALL colliders under the mouse point
        RaycastHit2D[] hits = Physics2D.RaycastAll(originPoint, Vector2.zero, Mathf.Infinity, dirtLayer);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                DirtTile dirtTile = hit.collider.GetComponent<DirtTile>();
                if (dirtTile != null && dirtTile.gameObject.activeInHierarchy)
                {
                    dirtTile.CleanTile(currentToolType);
                    break; // Cleaned the tile under cursor, done for this frame
                }
            }
        }
    }

    public void SetToolType(ToolType newToolType)
    {
        currentToolType = newToolType;
    }
}