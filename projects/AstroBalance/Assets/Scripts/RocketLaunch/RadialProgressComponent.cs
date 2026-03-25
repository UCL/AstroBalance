using System.Collections;
using System.Collections.Generic;
using MyUILibrary;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class RadialProgressComponent : MonoBehaviour
{
    LaunchControl countdownController;
    RadialProgress m_RadialProgress;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        countdownController = FindFirstObjectByType<LaunchControl>();
        m_RadialProgress = new RadialProgress()
        {
            style =
            {
                position = Position.Absolute,
                left = 860,
                top = 225,
                width = 200,
                height = 200,
            },
        };
        m_RadialProgress.trackColour = Color.red;

        root.Add(m_RadialProgress);
    }

    void Update()
    {
        m_RadialProgress.progress = countdownController.GetProgress();
        if (countdownController.GetProgress() >= 100)
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root.Remove(m_RadialProgress);
            this.enabled = false;
        }
    }
}
