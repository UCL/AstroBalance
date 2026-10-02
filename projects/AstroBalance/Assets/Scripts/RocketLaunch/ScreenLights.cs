using System.Collections.Generic;
using UnityEngine;

public class ScreenLights : MonoBehaviour
{
    private List<GameObject> Lights = new();
    private int LightsOn = 0;
    private LaunchControl countdownController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdownController = FindFirstObjectByType<LaunchControl>();
        foreach (Transform child in transform)
        {
            if (child.name.Substring(0, 5) == "Count")
            {
                Lights.Add(child.gameObject);
            }
        }

        foreach (GameObject light in Lights)
        {
            light.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetLights(countdownController.GetProgress() / 100f);
    }

    // Takes a fraction between 0 and 1 and turns on an appropriate number of lights.
    void SetLights(float f)
    {
        LightsOn = Mathf.FloorToInt(f * Lights.Count);
        if (LightsOn > Lights.Count)
            LightsOn = Lights.Count;
        for (int i = 0; i < LightsOn; i++)
        {
            Lights[i].SetActive(true);
        }
    }
}
