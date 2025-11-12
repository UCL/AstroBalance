using UnityEngine;
using static TrackerInterface;

public class rocket_control : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TrackerInterface.getGazePoint();
        Debug.Log("Rocket control update");
    }
}
