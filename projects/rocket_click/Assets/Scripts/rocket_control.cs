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
        transform.Translate(new Vector3(TrackerInterface.getGazePoint()[0]*0.01F, 0, 0));
        // Debug.Log("Rocket control update" + TrackerInterface.getGazePoint()[0]);
    }
}
