using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MyUILibrary;

[RequireComponent(typeof(UIDocument))]
public class RadialProgressComponent : MonoBehaviour
{
    LaunchControl coundownController;
    RadialProgress m_RadialProgress;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        coundownController = FindFirstObjectByType<LaunchControl>();
        m_RadialProgress = new RadialProgress() {
            style = {
                position = Position.Absolute,
                left = 850, top = 220, width = 200, height = 200,
            }
        };
        m_RadialProgress.trackColour = Color.red;

        root.Add(m_RadialProgress);
    }

    void Update()
    {
        m_RadialProgress.progress = coundownController.GetProgress();
    }
}
