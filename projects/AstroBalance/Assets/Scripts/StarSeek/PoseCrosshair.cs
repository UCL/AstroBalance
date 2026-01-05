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
        Vector2 headPointViewport = tracker.getHeadPointViewport();
        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(headPointViewport);

        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);
    }

}
