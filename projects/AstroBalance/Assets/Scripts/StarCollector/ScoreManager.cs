using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    private TextMeshProUGUI scoreText;
    private int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        score = 0;
        scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateScore()
    {
        score = score += 1;
        scoreText.text = score.ToString();
    }
}
