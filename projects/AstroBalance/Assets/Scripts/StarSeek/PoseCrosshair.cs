using UnityEngine;

public class PoseCrosshair : MonoBehaviour
{
    private Tracker tracker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 headPointWorld = tracker.getHeadWorldCoordinates();
        transform.position = new Vector3(headPointWorld.x, headPointWorld.y, transform.position.z);
    }
}
