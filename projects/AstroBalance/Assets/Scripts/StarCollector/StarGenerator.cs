using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("Default speed of generated stars (unity units / second)")]
    public float baseStarSpeed = 3f;

    [SerializeField, Tooltip("Maximum speed of generated stars (unity units / second)")]
    private float maxStarSpeed = 10f;

    [SerializeField, Tooltip("Minimum speed of generated stars (unity units / second)")]
    private float minStarSpeed = 2f;

    [
        SerializeField,
        Tooltip("Speed increment used when dynamically increasing or decreasing star speed")
    ]
    private float speedIncrement = 1f;

    [SerializeField, Tooltip("Number of stars to spawn per cycle of the wave")]
    private float starSampling = 10f;

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
    private float starCreationDistance; // y distance between spawned stars
    private bool isGenerating = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Calculate y distance between stars so there are 'starSampling' stars per cycle
        float yWavelength = (2 * Mathf.PI) / swerve;
        starCreationDistance = yWavelength / starSampling;

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
