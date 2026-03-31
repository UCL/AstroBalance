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
                left = new Length(45, LengthUnit.Percent),
                //top = new Length(20, LengthUnit.Percent), // this doesn't seem to work for setting the height.
		top = 225,
                width = new Length(10, LengthUnit.Percent),
                height = new Length(10, LengthUnit.Percent)
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
