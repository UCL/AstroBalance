using UnityEngine;

public class FlameController : MonoBehaviour
{
    LaunchControl launchController;

    [SerializeField, Tooltip("Magnitude of flame flicker effect")]
    private float flickerAmplitude = 0.15f;

    [SerializeField, Tooltip("Frequency of flame flicker effect")]
    private float flickerFrequency = 18f;
    
    [SerializeField, Tooltip("Flame scaling factor based on speed."), Range(0.01f, 0.5f)]
    private float flameSpeedScale = 0.025f;
    
    [SerializeField, Tooltip("Flame move factor based on speed."),
    Range(0.2f, 10.0f)]
    private float flameSpeedMove = 2.0f;


    private SpriteRenderer Renderer;
    private Vector3 original_scale;
    private Vector3 original_position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        launchController = FindFirstObjectByType<LaunchControl>();
        Renderer = GetComponent<SpriteRenderer>();
        original_scale = Renderer.transform.localScale;
        original_position = Renderer.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
	float headSpeed = launchController.HeadSpeed;
	bool launchComplete = launchController.GetProgress() >= 100f ? true : false;
	
	Vector3 new_scale = new Vector3(
            original_scale.x + headSpeed * flameSpeedScale / 10,
            original_scale.y + flickerAmplitude * Mathf.Sin(Mathf.PI * flickerFrequency * Time.time) + headSpeed * flameSpeedScale,
            original_scale.z
        );
        Renderer.transform.localScale = new_scale;

	if ( ! launchComplete )
	{
            Vector3 new_position = new Vector3(
                original_position.x ,
                original_position.y - headSpeed * flameSpeedMove/100,
                original_position.z
            );
	    Renderer.transform.position = new_position; 
	}
    }
}
