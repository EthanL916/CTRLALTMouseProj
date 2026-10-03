using UnityEngine;
using UnityEngine.InputSystem;

public class CleaningCursor : MonoBehaviour
{
    [Header("Tool Settings")]
    public ToolType currentTool;

    [Header("Tool Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Cursor Speed & Friction")]
    [Tooltip("How fast the cursor follows the mouse while holding left click to clean (Lower = heavier friction)")]
    [SerializeField] private float cleaningFollowSpeed = 12f;
    [Tooltip("If true, the cursor snaps 1:1 with the mouse when NOT cleaning.")]
    [SerializeField] private bool instantWhenNotCleaning = true;
    [Tooltip("Follow speed when not cleaning (only used if Instant When Not Cleaning is false)")]
    [SerializeField] private float normalFollowSpeed = 35f;

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

        Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        targetWorldPos.z = 0f;

        // Detect if left click is held down (cleaning state)
        bool isCleaning = Mouse.current.leftButton.isPressed;

        if (isCleaning)
        {
            // Smoothly drag the cursor toward target mouse position (adds friction feel)
            transform.position = Vector3.Lerp(transform.position, targetWorldPos, cleaningFollowSpeed * Time.deltaTime);
        }
        else
        {
            if (instantWhenNotCleaning)
            {
                // Snap directly to mouse position
                transform.position = targetWorldPos;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetWorldPos, normalFollowSpeed * Time.deltaTime);
            }
        }
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

    public void UpdateCleaningSprite(Sprite newCleaningTool, ToolType newTool)
    {
        currentTool = newTool;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newCleaningTool;
        }
    }
}
