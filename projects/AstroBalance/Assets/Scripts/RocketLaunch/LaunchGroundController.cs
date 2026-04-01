using UnityEngine;

public class LaunchGroundScroller : MonoBehaviour
{

    private Vector3 startPosition;

    LaunchControl launchController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	launchController = FindFirstObjectByType<LaunchControl>();
	startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
	transform.position = startPosition + (Vector3.down * launchController.Altitude);
    }
}
