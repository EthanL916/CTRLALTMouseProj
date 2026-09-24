using System.Collections;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [Header("Object Sides (Assign 6 Objects)")]
    [Tooltip("0: Front, 1: Right, 2: Back, 3: Left, 4: Top, 5: Bottom")]
    public GameObject[] objectSides;

    [Header("Flip Animation")]
    public float flipDuration = 0.15f;

    [Header("Swipe Settings")]
    public bool enableSwipe = true;
    public float swipeThreshold = 50f;

    private int currentSideIndex = 0; // Starts at Front (0)
    private int lastHorizontalIndex = 0; // Remembers Front(0), Right(1), Back(2), or Left(3)
    private bool isAnimating = false;

    private enum SwipeDirection { Left, Right, Up, Down }

    void Start()
    {
        UpdateVisibleSideImmediate();
    }

    public void RotateLeft() => TriggerRotation(SwipeDirection.Left, true);
    public void RotateRight() => TriggerRotation(SwipeDirection.Right, true);
    public void RotateUp() => TriggerRotation(SwipeDirection.Up, false);
    public void RotateDown() => TriggerRotation(SwipeDirection.Down, false);

    private void TriggerRotation(SwipeDirection dir, bool isHorizontal)
    {
        if (isAnimating) return;
        int nextSide = GetNextSide(currentSideIndex, dir);
        StartCoroutine(AnimateFlip(nextSide, isHorizontal));
    }

    private int GetNextSide(int current, SwipeDirection dir)
    {
    switch (current)
        {
        // 0: FRONT
        case 0:
            if (dir == SwipeDirection.Left) return 1;  // Right
            if (dir == SwipeDirection.Right) return 3; // Left
            if (dir == SwipeDirection.Up) return 4;    // Top
            if (dir == SwipeDirection.Down) return 5;  // Bottom
            break;

        // 1: RIGHT
        case 1:
            if (dir == SwipeDirection.Left) return 2;  // Back
            if (dir == SwipeDirection.Right) return 0; // Front
            if (dir == SwipeDirection.Up) return 4;    // Top
            if (dir == SwipeDirection.Down) return 5;  // Bottom
            break;

        // 2: BACK (Inverted relative to Front)
        case 2:
            if (dir == SwipeDirection.Left) return 3;  // Left
            if (dir == SwipeDirection.Right) return 1; // Right
            if (dir == SwipeDirection.Up) return 5;    // Bottom (Swiping UP on Back moves to Bottom!)
            if (dir == SwipeDirection.Down) return 4;  // Top (Swiping DOWN on Back moves to Top!)
            break;

        // 3: LEFT
        case 3:
            if (dir == SwipeDirection.Left) return 0;  // Front
            if (dir == SwipeDirection.Right) return 2; // Back
            if (dir == SwipeDirection.Up) return 4;    // Top
            if (dir == SwipeDirection.Down) return 5;  // Bottom
            break;

        // 4: TOP
        case 4:
            if (dir == SwipeDirection.Down) return 0;  // Pull down to Front
            if (dir == SwipeDirection.Up) return 2;    // Push up to Back
            if (dir == SwipeDirection.Left) return 1;  // Right
            if (dir == SwipeDirection.Right) return 3; // Left
            break;

        // 5: BOTTOM
        case 5:
            if (dir == SwipeDirection.Up) return 0;    // Push up to Front
            if (dir == SwipeDirection.Down) return 2;  // Pull down to Back
            if (dir == SwipeDirection.Left) return 1;  // Right
            if (dir == SwipeDirection.Right) return 3; // Left
            break;
        }

        return current;
    }

    private IEnumerator AnimateFlip(int targetIndex, bool horizontal)
    {
        if (targetIndex == currentSideIndex) yield break;

        isAnimating = true;

        float timer = 0f;
        Vector3 startScale = transform.localScale;
        
        Vector3 edgeOnScale = horizontal ? 
            new Vector3(0f, startScale.y, startScale.z) : 
            new Vector3(startScale.x, 0f, startScale.z);

        // 1. Squeeze Flat
        while (timer < flipDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, edgeOnScale, timer / flipDuration);
            yield return null;
        }

        // Store the last horizontal face index BEFORE changing if current is horizontal
        if (currentSideIndex < 4)
        {
            lastHorizontalIndex = currentSideIndex;
        }

        // 2. Switch Visible Face
        currentSideIndex = targetIndex;
        UpdateVisibleSideImmediate();

        // 3. Expand Back Out
        timer = 0f;
        while (timer < flipDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(edgeOnScale, startScale, timer / flipDuration);
            yield return null;
        }

        transform.localScale = startScale;
        isAnimating = false;
    }

    private void UpdateVisibleSideImmediate()
    {
        for (int i = 0; i < objectSides.Length; i++)
        {
            if (objectSides[i] != null)
            {
                bool isActive = (i == currentSideIndex);
                objectSides[i].SetActive(isActive);

                if (isActive)
                {
                    objectSides[i].transform.localPosition = Vector3.zero;
                }
            }
        }

        Physics2D.SyncTransforms();
    }
}