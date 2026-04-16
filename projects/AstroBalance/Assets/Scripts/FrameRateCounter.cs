using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrameRateCounter : MonoBehaviour
{
    List<float> rates = new(100);
    int i = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"i = {i}");
        if (i == 100)
        {
            float frate = rates.Average();
            Debug.Log($"Framerate = {frate}");
            rates = new(100);
            i = 0;
        }
        else
        {
            rates[i] = 1f / (Time.deltaTime);
            i += 1;
        }
    }
}
