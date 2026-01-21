using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LaunchCode : MonoBehaviour
{
    private float timerDuration = 1.0F;
    private float counter = 1.0F;
    private Sprite countDownSprite;
    [SerializeField, Tooltip("Sprites to display on the countdown.")]
    private List<Sprite> countDownSprites;

    [SerializeField]
    private TextMeshProUGUI countDownText;

    void Start()
    {
        countDownSprite = countDownSprites[Random.Range(0, countDownSprites.Count)];
        // remove the number from the list to avoid selected a repeat number next time.
        countDownSprites.Remove(countDownSprite);
        gameObject.GetComponent<SpriteRenderer>().sprite = countDownSprite;
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

            Sprite newCountDownSprite = countDownSprites[Random.Range(0, countDownSprites.Count)];
            // remove the number from the list to avoid selected a repeat number next time.
            countDownSprites.Remove(newCountDownSprite);
            countDownSprites.Add(countDownSprite);
            countDownSprite = newCountDownSprite;
            gameObject.GetComponent<SpriteRenderer>().sprite = newCountDownSprite;
        }
    }
}
