using UnityEngine;
using UnityEngine.InputSystem;

public class CleaningCursor : MonoBehaviour
{
    [Header("Tool Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Cursor.visible = false;
    }

    private void Update()
    {
        FollowMousePointer();
    }

    private void FollowMousePointer()
    {
        if (Camera.main == null || Mouse.current == null) return;

        Vector2 mouseScreenPos2D = Mouse.current.position.ReadValue();
        Vector3 mouseScreenPos = new Vector3(mouseScreenPos2D.x, mouseScreenPos2D.y, 0f);

        // Set camera depth offset
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        worldPos.z = 0f;

        transform.position = worldPos;
    }

    public void UpdateCleaningSprite(Sprite newCleaningTool)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newCleaningTool;
        }
    }
}
