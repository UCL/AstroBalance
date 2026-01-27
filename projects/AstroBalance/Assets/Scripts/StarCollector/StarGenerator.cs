using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("Default speed of generated stars")]
    public float baseStarSpeed = 3f;

    [SerializeField, Tooltip("Maximum speed of generated stars")]
    private float maxStarSpeed = 10f;

    [SerializeField, Tooltip("Minimum speed of generated stars")]
    private float minStarSpeed = 2f;

    [
        SerializeField,
        Tooltip("Speed increment used when dynamically increasing or decreasing star speed")
    ]
    private float speedIncrement = 1f;

    [SerializeField, Tooltip("Distance between generated stars on the y axis")]
    private float starCreationDistance = 2.5f;

    [
        SerializeField,
        Tooltip("Higher swerve generates a wave that oscillates from left to right more often")
    ]
    private float swerve = 0.1f;

    [SerializeField, Tooltip("Width of the generated wave of stars on the x axis")]
    private float waveWidth = 3f;

    [SerializeField, Tooltip("Star game object to generate")]
    private GameObject starPrefab;

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
        while (d <= pathDistance)
        {
            CreateStar(d);
            d += starCreationDistance;
        }
    }

    private void CreateStar(float d)
    {
        var starObject = Instantiate(
            starPrefab,
            new Vector3(Mathf.Sin(d_eff * swerve) * waveWidth, d, 0),
            Quaternion.identity
        );
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
        if (frontier >= starCreationDistance)
        {
            frontier -= starCreationDistance;
            CreateStar(pathDistance - frontier);
        }
    }
}
