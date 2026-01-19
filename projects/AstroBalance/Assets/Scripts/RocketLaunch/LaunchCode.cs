using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class LaunchCode: MonoBehaviour
{
    private float timerDuration = 1.0F;
    private float counter = 1.0F;
    private string countDownNumber;
    private List<string> countDownNumbers = new List<string> {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};

    private TextMeshProUGUI countDownText;
    void Start()
    {

        countDownNumber = countDownNumbers[Random.Range(0, countDownNumbers.Count)];
        // remove the number from the list to avoid selected a repeat number next time.
        countDownNumbers.Remove(countDownNumber);
    }
    void Update()
    {
        if (counter > 0)
        {
            counter -= Time.deltaTime;
        }
        else
        {
            counter = timerDuration;
            string newCountDownNumber = countDownNumbers[Random.Range(0, countDownNumbers.Count)];
            countDownNumbers.Remove(newCountDownNumber);
            countDownNumbers.Add(countDownNumber);
            countDownNumber = newCountDownNumber;
            countDownText.text = countDownNumber;
            
        }
    }
}

