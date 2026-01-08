using TMPro;
using UnityEngine;

public class StarSeekManager : MonoBehaviour
{
    [SerializeField, Tooltip("Text mesh pro object for score text")]
    private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("Text mesh pro object for countdown timer text")]
    private TextMeshProUGUI timerText;
    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;
    [SerializeField, Tooltip("Game time limit in seconds")]
    private int timeLimit = 120;

    private int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
