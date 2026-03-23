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
                left = 200, top = 100, width = 200, height = 200,
            }
        };
        m_RadialProgress.m_ProgressColor = Color.red;

        root.Add(m_RadialProgress);
    }

    void Update()
    {
        m_RadialProgress.progress = coundownController.GetProgress();
    }
}
