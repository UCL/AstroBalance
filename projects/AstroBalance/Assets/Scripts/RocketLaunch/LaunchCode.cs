using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class LaunchCode: MonoBehaviour
{
    private float timerDuration = 1.0F;
    private float counter = 1.0F;
    private int countDownNumber = 0;

    public TextMeshProUGUI countDownText;
    void Start()
    {
        countDownNumber = Random.Range(0, 10);
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
            int newCountDownNumber = Random.Range(0, 10);
            // for the purposes of the game we want to make sure that the 
            // time changes every second, so keep trying random until
            // we get a new one.
            while (newCountDownNumber == countDownNumber)
                newCountDownNumber = Random.Range(0, 10);
            countDownNumber = newCountDownNumber;
            UnityEngine.Debug.Log(countDownNumber.ToString());
            countDownText.text = countDownNumber.ToString();
            
        }
    }
}

