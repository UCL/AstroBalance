using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MyUILibrary;

[RequireComponent(typeof(UIDocument))]
public class RadialProgressComponent : MonoBehaviour
{

    RadialProgress m_RadialProgress;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        m_RadialProgress = new RadialProgress() {
            style = {
                position = Position.Absolute,
                left = 20, top = 20, width = 200, height = 200
            }
        };

        root.Add(m_RadialProgress);
    }

    void Update()
    {
        // For demo purpose, give the progress property dynamic values.
        m_RadialProgress.progress = ((Mathf.Sin(Time.time) + 1.0f) / 2.0f) * 60.0f + 10.0f;
    }
}
