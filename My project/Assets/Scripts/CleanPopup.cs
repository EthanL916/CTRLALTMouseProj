using UnityEngine;
using TMPro; 

public class CleanPopup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float lifetime = 1.5f; 
    [SerializeField] private TextMeshPro textComponent;

    [Header("Visuals")]
    [SerializeField] private Vector3 popScaleAmount = new Vector3(1.3f, 1.3f, 1.0f);

    private float timer = 0f;
    private Color startTextColor;

    private void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshPro>();
        }

        if (textComponent != null)
        {
            startTextColor = textComponent.color;
        }

        transform.localScale = Vector3.one * 0.5f;
    }

    private void Update()
    {
        if (textComponent == null) return;

        timer += Time.deltaTime;
        float normalizedTime = timer / lifetime; 

        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        if (normalizedTime < 0.2f)
        {
            float growProgress = normalizedTime / 0.2f;
            transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, popScaleAmount, growProgress);
        }
        else
        {
            float shrinkProgress = (normalizedTime - 0.2f) / 0.8f;
            transform.localScale = Vector3.Lerp(popScaleAmount, Vector3.one, shrinkProgress);
        }

        float newAlpha = Mathf.Lerp(startTextColor.a, 0f, normalizedTime);
        textComponent.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, newAlpha);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}