using System.Collections;
using TMPro;
using Tobii.GameIntegration.Net;
using UnityEngine;
using UnityEngine.UI;

public class LaunchProgressRing : MonoBehaviour
{
    [SerializeField, Tooltip("Arrow fill colour")]
    private Color fillColor = Color.red;

    LaunchControl countdownController;

    private Image fillImage;
    private float delaySeconds = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdownController = FindFirstObjectByType<LaunchControl>();
        Image[] ringImages = GetComponentsInChildren<Image>();

        foreach (Image image in ringImages)
        {
            if (image.type == Image.Type.Filled)
            {
                fillImage = image;
                break;
            }
        }

        fillImage.color = fillColor;
    }

    // Update is called once per frame
    void Update()
    {
        float progress = countdownController.GetProgress() / 100f;
        if (progress < 1f)
        {
            fillImage.fillAmount = progress;
        }
        else
        {
            StartCoroutine(HandleLaunchComplete());
        }
    }

    private IEnumerator HandleLaunchComplete()
    {
        yield return new WaitForSeconds(delaySeconds);
        Destroy(gameObject);
    }
}
