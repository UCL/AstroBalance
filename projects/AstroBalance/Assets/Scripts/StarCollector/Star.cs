using System.Collections;
using UnityEngine;

public class Star : MonoBehaviour
{
    public GameObject sparkleEffect;
    public StarGenerator starGenerator;
   
    private StarCollectorManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("StarCollectorManager").GetComponent<StarCollectorManager>();
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
        if (!gameManager.IsGameActive())
        {
            return;
        }

        
        if (other.CompareTag("Player"))
        {
            // If the star had touched the player (i.e. the ship)
            GameObject deathSparkle = Instantiate<GameObject>(sparkleEffect);
            deathSparkle.transform.position = transform.position;
            Destroy(deathSparkle, 1.0f);

            Destroy(gameObject);

            gameManager.UpdateScore();
        } 
        else
        {
            // If the star had touched the MissedStarDetector
            gameManager.UpdateMisses();
        }
    }
}
