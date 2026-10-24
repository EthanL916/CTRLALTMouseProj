using UnityEngine;
using UnityEngine.InputSystem;

public class CleaningCursor : MonoBehaviour
{
    [Header("Tool Settings")]
    public ToolType currentTool;

    [Header("Tool Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer secondaryOverlayRenderer;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem bubbleParticles;

    [Header("Cursor Speed Settings")]
    [SerializeField] private float cleaningFollowSpeed = 12f;
    [SerializeField] private bool instantWhenNotCleaning = true;
    [SerializeField] private float normalFollowSpeed = 35f;

    [Header("Rotation Settings")]
    [SerializeField] private ObjectRotator objectRotator;
    [SerializeField] private float swipeThreshold = 60f;

    private Sprite normalSprite;
    private Sprite activeSprite;

    private Vector2 swipeStartPosition;
    private bool isSwiping = false;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (objectRotator == null)
        {
            objectRotator = Object.FindFirstObjectByType<ObjectRotator>();
        }

        // Hide mouse pointer
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

        // Idle Sprite
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

        // Cleaning Sprite
        if (secondaryOverlayRenderer != null)
        {
            secondaryOverlayRenderer.enabled = isCleaning;
        }
    }

    private void HandleSwipeInput()
    {
        if (Mouse.current == null || objectRotator == null) return;

        // Right Click Down
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            swipeStartPosition = Mouse.current.position.ReadValue();
            isSwiping = true;
        }

        // Right Click Release
        if (Mouse.current.rightButton.wasReleasedThisFrame && isSwiping)
        {
            Vector2 swipeEndPosition = Mouse.current.position.ReadValue();
            Vector2 delta = swipeEndPosition - swipeStartPosition;

            if (delta.magnitude >= swipeThreshold)
            {
                float xDistance = Mathf.Abs(delta.x);
                float yDistance = Mathf.Abs(delta.y);

                if (xDistance > yDistance)
                {
                    if (delta.x < 0)
                    {
                        objectRotator.RotateRight();
                    }
                    else
                    {
                        objectRotator.RotateLeft();
                    }
                }
                else
                {
                    if (delta.y > 0)
                    {
                        objectRotator.RotateUp();
                    }
                    else
                    {
                        objectRotator.RotateDown();
                    }
                }
            }

            isSwiping = false;
        }
    }

    // Switch tools
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

    // Bubble particles
    public void ToggleBubbles(bool shouldEmit)
    {
        if (bubbleParticles == null) return;

        var emission = bubbleParticles.emission;
        emission.enabled = shouldEmit;

        if (shouldEmit)
        {
            if (bubbleParticles.isPlaying == false)
            {
                bubbleParticles.Play();
            }
        }
    }
}
