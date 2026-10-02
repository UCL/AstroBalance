using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [SerializeField, Tooltip("Ordered list of components to turn on during tutorial")]
    private List<GameObject> ComponentSequence;

    [SerializeField, Tooltip("Ordered list of events to trigger during the tutorial.")]
    private List<UnityEvent> EventSequence;

    [SerializeField, Tooltip("Ordered sequence of instructions.")]
    private List<string> Instructions;

    [SerializeField, Tooltip("Instruction Text Object")]
    private TextMeshProUGUI InstructionText;

    private int NumStates;
    private int CurrentState;

    private SceneSelector sceneSelector;

    [SerializeField, Tooltip("Which scene to load when the tutorial is done.")]
    private UnityEvent endTutorial;

    enum State
    {
        Initial,
        Final,
    }

    private void ResizeList<T>(List<T> L, int N, T pad)
    {
        if (L.Count < N)
        {
            var d = N - L.Count;
            for (int i = 0; i < d; i++)
            {
                L.Add(pad);
            }
        }
        else if (L.Count > N)
        {
            var d = L.Count - N;
            L.RemoveRange(N, d);
        }
        return;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneSelector = FindFirstObjectByType<SceneSelector>();
        NumStates = Instructions.Count;
        foreach (var obj in ComponentSequence)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        CurrentState = 0;
        InstructionText.text = Instructions[CurrentState];

        ResizeList(ComponentSequence, Instructions.Count, null);
        ResizeList(EventSequence, Instructions.Count, null);
    }

    // Update is called once per frame
    void Update() { }

    public void AdvanceInstructions()
    {
        CurrentState += 1;
        if (CurrentState >= NumStates)
        {
            endTutorial.Invoke();
        }
        else
        {
            InstructionText.text = Instructions[CurrentState];
            if (ComponentSequence[CurrentState] != null)
            {
                ComponentSequence[CurrentState].SetActive(true);
            }
            if (EventSequence[CurrentState] != null)
            {
                EventSequence[CurrentState].Invoke();
            }
        }
    }
}
