using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    public float baseStarSpeed = 2f;
    public float starCreationDistance = 2.5f;
    public float swerve = 0.1f;
    public float waveWidth = 3f;
    public float speedIncrement = 2f;
    public GameObject starPrefab;

    private StarCollectorManager gameManager;
    private float pathDistance = 15f;
    private float frontier = 0;
    private float d_eff;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("StarCollectorManager").GetComponent<StarCollectorManager>();
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
        star.speed = baseStarSpeed;
    }

    public void increaseSpeed()
    {
        baseStarSpeed += speedIncrement;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.isGameActive())
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
