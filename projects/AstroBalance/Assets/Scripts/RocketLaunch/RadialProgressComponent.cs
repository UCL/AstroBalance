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
                left = new Length(44, LengthUnit.Percent),
                width = new Length(12, LengthUnit.Percent),
                height = new Length(12, LengthUnit.Percent),
            },
        };
        m_RadialProgress.trackColour = Color.red;

	// Claude (Sonnet 4.6) came up with the following in order to get the height right. It works better than
	// anything else I've found but is still not quite right.
        // ScaleWithScreenSize + match=0 (width): logical panel height = Screen.height * refWidth / Screen.width.
        // Display scaling cancels out because both Screen dimensions scale by the same factor.
        var panelSettings = GetComponent<UIDocument>().panelSettings;
        float panelHeight = Screen.height * panelSettings.referenceResolution.x / Screen.width;
        m_RadialProgress.style.top = panelHeight * 0.480f;

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
