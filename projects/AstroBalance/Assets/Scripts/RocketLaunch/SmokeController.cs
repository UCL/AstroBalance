using UnityEngine;

public class SmokeController : MonoBehaviour
{
    [SerializeField, Tooltip("The rocket launch object")]
    LaunchControl launchController;

    private ParticleSystem smokeEmitter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        smokeEmitter = GetComponent<ParticleSystem>();
	var emitter = smokeEmitter.emission;
	emitter.rateOverTime = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {
	float headSpeed = launchController.HeadSpeed;
	bool launchComplete = launchController.GetProgress() >= 100f ? true : false;
	

	if ( ! launchComplete )
	{
	    var emitter = smokeEmitter.emission;
	    emitter.rateOverTime = headSpeed / 20f;
	}
    }
}
