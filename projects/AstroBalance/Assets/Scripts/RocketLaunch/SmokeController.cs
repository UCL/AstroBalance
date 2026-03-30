using UnityEngine;

public class SmokeController : MonoBehaviour
{
    [SerializeField, Tooltip("The rocket launch object")]
    LaunchControl launchController;

    private ParticleSystem[] smokeEmitters;

    public ParticleSystem smoke_1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        smokeEmitters = GetComponents<ParticleSystem>();
	Debug.Log("There are " + smokeEmitters.Length + " emitters");
	for (int i = 0; i < smokeEmitters.Length; i++)
	{
	    var emitter = smokeEmitters[i].emission;
	    emitter.rateOverTime = 0f;
	}
        
    }

    // Update is called once per frame
    void Update()
    {
	float headSpeed = launchController.HeadSpeed;
	bool launchComplete = launchController.GetProgress() >= 100f ? true : false;
	

	if ( ! launchComplete )
	{
	    for (int i = 0; i < smokeEmitters.Length; i++)
	    {
	        var emitter = smokeEmitters[i].emission;
	        emitter.rateOverTime = headSpeed/20f;
	    }
	}
    }
}
