using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class DirtTile : MonoBehaviour
{
    [Header("Tile Health/Cleanliness")]
    [SerializeField] private float maxDirtiness = 100f;
    [SerializeField] private float currentDirtiness = 100f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject cleanPopupPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Tool Compatibility")]
    [Tooltip("Which cleaning tool is effective against this specific dirt spot?")]
    [SerializeField] public ToolType requiredTool = ToolType.Sponge;

    private bool isFullyCleaned = false;

    public float MaxDirtiness => maxDirtiness;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        currentDirtiness = maxDirtiness;
    }

    public void CleanTile(ToolType usedTool)
    {
        if (isFullyCleaned || usedTool != requiredTool || currentDirtiness <= 0f) return;

        float previousDirtiness = currentDirtiness;

        currentDirtiness -= Time.deltaTime * 50f;
        currentDirtiness = Mathf.Clamp(currentDirtiness, 0f, maxDirtiness);

        float cleanedAmount = previousDirtiness - currentDirtiness;

        DirtTracker.ReportCleaning(cleanedAmount);

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = currentDirtiness / maxDirtiness;
            spriteRenderer.color = color;
        }

        if (currentDirtiness <= 0f)
        {
            isFullyCleaned = true;

            if (cleanPopupPrefab != null)
            {
                Instantiate(cleanPopupPrefab, transform.position, Quaternion.identity);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayCleanedSound();
            }

            gameObject.SetActive(false);
        }
    }
}
