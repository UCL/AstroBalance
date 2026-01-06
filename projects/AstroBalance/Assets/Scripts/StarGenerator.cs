using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    [Tooltip("Default speed of generated stars")]
    public float baseStarSpeed = 3f;
    [Tooltip("Maximum speed of generated stars")]
    public float maxStarSpeed = 10f;
    [Tooltip("Minimum speed of generated stars")]
    public float minStarSpeed = 2f;
    [Tooltip("Speed increment used when dynamically increasing or decreasing star speed")]
    public float speedIncrement = 1f;

    [Tooltip("Distance between generated stars on the y axis")]
    public float starCreationDistance = 2.5f;
    [Tooltip("Higher swerve generates a wave that oscillates from left to right more often")]
    public float swerve = 0.1f;
    [Tooltip("Width of the generated wave of stars on the x axis")]
    public float waveWidth = 3f;
    [Tooltip("Star game object to generate")]
    public GameObject starPrefab;

    private float pathDistance = 15f;
    private float frontier = 0;
    private float d_eff;
    private bool isGenerating = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitStars();
    }

    void InitStars()
    {
        float d = 0;
        while(d <= pathDistance)
        {
            CreateStar(d);
            d += starCreationDistance;
        }
    }

    private void CreateStar(float d)
    {
        var starObject = Instantiate(starPrefab, new Vector3(Mathf.Sin(d_eff * swerve) * waveWidth, d, 0), Quaternion.identity);
        d_eff += starCreationDistance;
        var star = starObject.GetComponent<Star>();
        star.starGenerator = this;
    }

    public void IncreaseSpeed()
    {
        UpdateSpeed(speedIncrement);
    }

    public void DecreaseSpeed()
    {
        UpdateSpeed(-speedIncrement);
    }

    private void UpdateSpeed(float increment)
    {
        float nextSpeed = baseStarSpeed + increment;
        if (nextSpeed > maxStarSpeed)
        {
            baseStarSpeed = maxStarSpeed;
        }
        else if (nextSpeed < minStarSpeed)
        {
            baseStarSpeed = minStarSpeed;
        }
        else
        {
            baseStarSpeed = nextSpeed;
        }
    }

    public void StopGeneration()
    {
        isGenerating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGenerating)
        {
            return;
        }

        frontier += baseStarSpeed * Time.deltaTime;
        if(frontier >= starCreationDistance)
        {
            frontier -= starCreationDistance;
            CreateStar(pathDistance - frontier);
        }
    }

}
