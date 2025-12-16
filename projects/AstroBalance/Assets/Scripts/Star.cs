using System.Collections;
using UnityEngine;

public class Star : MonoBehaviour
{
    public float speed;
    public StarGenerator starGenerator;
    [SerializeField] public GameObject sparkleEffect;

    private ScoreManager scoreManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreManager = GameObject.Find("Score_text").GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y - starGenerator.baseStarSpeed * Time.deltaTime, pos.z);

        if (pos.y < -10)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject deathSparkle = Instantiate<GameObject>(sparkleEffect);
        deathSparkle.transform.position = transform.position;
        Destroy(deathSparkle, 1.0f);
        
        Destroy(gameObject);

        scoreManager.updateScore();
    }
}
