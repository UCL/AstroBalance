using UnityEngine;
using Tobii.GameIntegration.Net;

public class Ship : MonoBehaviour
{
    Tracker tracker;

    [SerializeField] private float vByDegrees = 1f;
    [SerializeField] private float xByDegrees = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
    }

    private void MoveByVelocity()
    {
        float yaw = tracker.getHeadRotation().YawDegrees;
        float vx = yaw * vByDegrees;
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x + vx * Time.deltaTime, pos.y, pos.z);
    }

    private void Move()
    {
        float yaw = tracker.getHeadRotation().YawDegrees;
        float x = yaw * xByDegrees;
        Vector3 pos = transform.position;
        transform.position = new Vector3(x, pos.y, pos.z);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}
