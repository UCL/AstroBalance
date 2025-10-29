
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketScript : MonoBehaviour
{
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
