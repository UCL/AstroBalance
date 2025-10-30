
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;

public class RocketScript : MonoBehaviour
{
    #if UNITY_STANDALONE_WIN
    [DllImport("tobii_gameintegration_x64.dll")]
    #endif
    private static extern float GetEyePositionX();
    private static extern float GetEyePositionY();
    private static extern float GetEyePositionZ();

    void Start()
    {
        Debug.Log("Hello World");
    }

    void Update()
    {
        var gamepad = Gamepad.current; //swap this with Tobii when we have it.
        var mouse = Mouse.current;
        if (gamepad == null)
        {
            if (mouse == null)
            {
                Debug.Log("Update no input");
            }
            else
            {
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    Debug.Log("Mouse pressed");
                }
                else if (mouse.leftButton.wasReleasedThisFrame)
                {
                    Debug.Log("Mouse released");
                }
            }

        }
    }
}
