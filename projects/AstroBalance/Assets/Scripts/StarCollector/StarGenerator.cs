using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    public float baseStarSpeed = 3f;
    public float maxStarSpeed = 10f;
    public float minStarSpeed = 2f;
    public float speedIncrement = 1f;

    public float starCreationDistance = 2.5f;
    public float swerve = 0.1f;
    public float waveWidth = 3f;
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
