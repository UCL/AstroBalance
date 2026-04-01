using UnityEngine;

public class LaunchBackgroundScroller : MonoBehaviour
{

    private Material mat;

    LaunchControl launchController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
	launchController = FindFirstObjectByType<LaunchControl>();
    }

    // Update is called once per frame
    void Update()
    {
        mat.mainTextureOffset = new Vector2(0, launchController.Altitude);
    }
}
