using UnityEngine;
using UnityEngine.InputSystem;

public class CleaningCursor : MonoBehaviour
{
    [Header("Tool Settings")]
    public ToolType currentTool;

    [Header("Tool Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Optional: Secondary overlay SpriteRenderer (e.g., foam or bubbles child object) active while cleaning")]
    [SerializeField] private SpriteRenderer secondaryOverlayRenderer;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem bubbleParticles;

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

    private Sprite normalSprite;
    private Sprite activeSprite;

    private Vector2 swipeStartPosition;
    private bool isSwiping = false;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (objectRotator == null)
            objectRotator = Object.FindFirstObjectByType<ObjectRotator>();

        Cursor.visible = false;
    }

    private void Update()
    {
        FollowMousePointer();
        HandleSwipeInput();
        UpdateVisuals();
    }

    private void FollowMousePointer()
    {
        if (Camera.main == null || Mouse.current == null) return;

        Vector2 mouseScreenPos2D = Mouse.current.position.ReadValue();
        Vector3 mouseScreenPos = new Vector3(mouseScreenPos2D.x, mouseScreenPos2D.y, 0f);

        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        targetWorldPos.z = 0f;

        bool isCleaning = Mouse.current.leftButton.isPressed;

        if (isCleaning)
        {
            transform.position = Vector3.Lerp(transform.position, targetWorldPos, cleaningFollowSpeed * Time.deltaTime);
        }
        else
        {
            if (instantWhenNotCleaning)
            {
                transform.position = targetWorldPos;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetWorldPos, normalFollowSpeed * Time.deltaTime);
            }
        }
    }

    private void UpdateVisuals()
    {
        if (Mouse.current == null) return;

        bool isCleaning = Mouse.current.leftButton.isPressed;

        // Swap main tool sprite between idle and active cleaning state
        if (spriteRenderer != null)
        {
            if (isCleaning && activeSprite != null)
            {
                spriteRenderer.sprite = activeSprite;
            }
            else if (normalSprite != null)
            {
                spriteRenderer.sprite = normalSprite;
            }
        }

        // Toggle optional secondary overlay graphics (e.g. soap bubbles or splashes)
        if (secondaryOverlayRenderer != null)
        {
            secondaryOverlayRenderer.enabled = isCleaning;
        }
    }

    private void HandleSwipeInput()
    {
        if (Mouse.current == null || objectRotator == null) return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            swipeStartPosition = Mouse.current.position.ReadValue();
            isSwiping = true;
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame && isSwiping)
        {
            Vector2 swipeEndPosition = Mouse.current.position.ReadValue();
            Vector2 delta = swipeEndPosition - swipeStartPosition;

            if (delta.magnitude >= swipeThreshold)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x < 0) objectRotator.RotateRight();
                    else objectRotator.RotateLeft();
                }
                else
                {
                    if (delta.y > 0) objectRotator.RotateUp();
                    else objectRotator.RotateDown();
                }
            }

            isSwiping = false;
        }
    }

    public void UpdateCleaningSprite(Sprite newCleaningTool, Sprite newActiveSprite, ToolType newTool)
    {
        currentTool = newTool;
        normalSprite = newCleaningTool;
        activeSprite = newActiveSprite;

        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    public void ToggleBubbles(bool shouldEmit)
{
    if (bubbleParticles == null) return;

    var emission = bubbleParticles.emission;
    emission.enabled = shouldEmit;

    if (shouldEmit && !bubbleParticles.isPlaying)
    {
        bubbleParticles.Play();
    }
}
}
