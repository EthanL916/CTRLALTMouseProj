using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
   
    DirtTile dirtTile;
    public GameObject DirtyTile;


    private Slider slider;
    private float setProgress = 0;
    public float fillSpeed = 0.5f;


    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
        dirtTile = DirtyTile.GetComponent<DirtTile>();
    }

    void Start()
    {
     // if (currentDirtiness <= 0f)
        {
            increaseProgress (0.25f);
        }
    }

    
    void Update()
    {
        if (slider.value < setProgress)
            slider.value += fillSpeed * Time.deltaTime;
    }

    //adding progress to slider
    public void increaseProgress(float newProgress)
    {
        setProgress = slider.value + newProgress;
    }

}
