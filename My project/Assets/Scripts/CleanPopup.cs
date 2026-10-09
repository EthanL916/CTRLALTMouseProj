using UnityEngine;
using TMPro; // Crucial for accessing TextMeshPro

public class CleanPopup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float lifetime = 0.75f;
    [SerializeField] private TextMeshPro textComponent;

    [Header("Visuals")]
    [SerializeField] private Vector3 popScaleAmount = new Vector3(1.3f, 1.3f, 1.0f);

    private float timer;
    private Color startTextColor;

    void Start()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshPro>();

        // Capture initial color to handle fading
        startTextColor = textComponent.color;

        // Optional: Start slightly smaller for a "pop" animation
        transform.localScale = Vector3.one * 0.5f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float normalizedTime = timer / lifetime;

        // 1. Move Up
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        // 2. Pop Animation (quickly grow then steady)
        if (normalizedTime < 0.2f)
        {
            // Grow quickly
            transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, popScaleAmount, normalizedTime / 0.2f);
        }
        else
        {
            // Subtle settle
            transform.localScale = Vector3.Lerp(popScaleAmount, Vector3.one, (normalizedTime - 0.2f) / 0.8f);
        }

        // 3. Fade Out Text Alpha
        float newAlpha = Mathf.Lerp(startTextColor.a, 0f, normalizedTime);
        textComponent.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, newAlpha);

        // 4. Auto-Destroy
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
