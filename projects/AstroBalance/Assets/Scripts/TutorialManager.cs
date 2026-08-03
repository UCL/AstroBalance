using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField, Tooltip("Ordered list of components to turn on during tutorial")]
    private List<GameObject> ComponentSequence;

    [SerializeField, Tooltip("Ordered sequence of instructions.")]
    private List<string> Instructions;

    [SerializeField, Tooltip("Instruction Text Object")]
    private TextMeshProUGUI InstructionText;

    private int NumStates;
    private int CurrentState;

    private SceneSelector sceneSelector;

    enum State
    {
        Initial,
        Final,
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
    }

    // Update is called once per frame
    void Update() { }

    public void AdvanceInstructions()
    {
        CurrentState += 1;
        if (CurrentState >= NumStates)
        {
            sceneSelector = this.AddComponent<SceneSelector>();
            sceneSelector.LoadRocketLaunchInstructions();
        }
        else
        {
            InstructionText.text = Instructions[CurrentState];
            if (ComponentSequence[CurrentState] != null)
            {
                ComponentSequence[CurrentState].SetActive(true);
            }
        }
    }
}
