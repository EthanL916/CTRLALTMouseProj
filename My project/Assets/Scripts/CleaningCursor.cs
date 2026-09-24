using UnityEngine;
using UnityEngine.InputSystem;

public class CleaningCursor : MonoBehaviour
{
    [Header("Tool Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Rotation Settings")]
    [SerializeField] private ObjectRotator objectRotator;
    [Tooltip("Minimum drag distance in pixels to trigger a side flip")]
    [SerializeField] private float swipeThreshold = 60f;

    private Vector2 swipeStartPosition;
    private bool isSwiping = false;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Auto-find ObjectRotator in scene if not assigned in Inspector
        if (objectRotator == null)
            objectRotator = Object.FindFirstObjectByType<ObjectRotator>();

        Cursor.visible = false;
    }

    private void Update()
    {
        FollowMousePointer();
        HandleSwipeInput();
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

    private void HandleSwipeInput()
    {
        if (Mouse.current == null || objectRotator == null) return;

        // Right mouse button pressed down
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            swipeStartPosition = Mouse.current.position.ReadValue();
            isSwiping = true;
        }

        // Right mouse button released
        if (Mouse.current.rightButton.wasReleasedThisFrame && isSwiping)
        {
            Vector2 swipeEndPosition = Mouse.current.position.ReadValue();
            Vector2 delta = swipeEndPosition - swipeStartPosition;

            // Check if the overall swipe vector exceeds the threshold
            if (delta.magnitude >= swipeThreshold)
            {
                // Check if horizontal drag was larger than vertical drag
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    // Horizontal Swipes
                    if (delta.x < 0)
                    {
                        objectRotator.RotateRight(); // Swiped Left -> rotate to view right side
                    }
                    else
                    {
                        objectRotator.RotateLeft();  // Swiped Right -> rotate to view left side
                    }
                }
                else
                {
                    // Vertical Swipes
                    if (delta.y > 0)
                    {
                        objectRotator.RotateUp();    // Swiped Up -> flip to top side
                    }
                    else
                    {
                        objectRotator.RotateDown();  // Swiped Down -> flip to bottom side
                    }
                }
            }

            isSwiping = false;
        }
    }

    public void UpdateCleaningSprite(Sprite newCleaningTool)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newCleaningTool;
        }
    }
}
