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
    VisualElement m_Container;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        countdownController = FindFirstObjectByType<LaunchControl>();

        // A full-screen container with an explicit 100%x100% size gives child elements
        // a definite reference for percentage-based top/left, which otherwise fails to
        // resolve when display scaling is active (panel logical size != Screen pixels).
        m_Container = new VisualElement();
        m_Container.style.position = Position.Absolute;
        m_Container.style.left = 0;
        m_Container.style.top = 0;
        m_Container.style.width = new Length(100, LengthUnit.Percent);
        m_Container.style.height = new Length(100, LengthUnit.Percent);

        m_RadialProgress = new RadialProgress()
        {
            style =
            {
                position = Position.Absolute,
                left = new Length(45, LengthUnit.Percent),
                // top = new Length(100, LengthUnit.Percent),
		top = 300,
                width = new Length(10, LengthUnit.Percent),
                height = new Length(10, LengthUnit.Percent),
            },
        };
        m_RadialProgress.trackColour = Color.red;

        m_Container.Add(m_RadialProgress);
        root.Add(m_Container);
    }

    void Update()
    {
        m_RadialProgress.progress = countdownController.GetProgress();
        if (countdownController.GetProgress() >= 100)
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            root.Remove(m_Container);
            this.enabled = false;
        }
    }
}
