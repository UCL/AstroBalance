using UnityEngine;

public class FlameController : MonoBehaviour
{
    LaunchControl launchController;

    [SerializeField, Tooltip("Magnitude of flame flicker effect")]
    private float flickerAmplitude = 0.15f;

    [SerializeField, Tooltip("Frequency of flame flicker effect")]
    private float flickerFrequency = 18f;

    [SerializeField, Tooltip("Flame scaling factor based on speed."), Range(0.001f, 0.1f)]
    private float flameSpeedScale = 0.0025f;

    [SerializeField, Tooltip("Flame move factor based on speed."), Range(0.002f, 0.20f)]
    private float flameSpeedMove = 0.02f;

    [SerializeField, Tooltip("Smoke emission scale"), Range(0f, 0.5f)]
    float smokeEmissionScale = 0.05f;
    private ParticleSystem[] smokeEmitters;

    private SpriteRenderer Renderer;
    private Vector3 original_scale;
    private Vector3 original_position;

    private AudioSource EngineAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        launchController = FindFirstObjectByType<LaunchControl>();
        Renderer = GetComponent<SpriteRenderer>();
        original_scale = Renderer.transform.localScale;
        original_position = Renderer.transform.localPosition;
        InitialiseSmokeEmitters();
        EngineAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float headSpeed = launchController.HeadSpeed;
        bool launchComplete = launchController.GetProgress() >= 100f ? true : false;
        float flameStrength = 0f;
        if (!launchComplete)
        {
            flameStrength = headSpeed;
        }
        else
        {
            flameStrength = 100.0f;
        }

        Vector3 new_scale = new Vector3(
            original_scale.x + headSpeed * flameSpeedScale / 10,
            original_scale.y
                + flickerAmplitude * Mathf.Sin(Mathf.PI * flickerFrequency * Time.time)
                + headSpeed * flameSpeedScale,
            original_scale.z
        );
        Renderer.transform.localScale = new_scale;

        Vector3 new_position = new Vector3(
            original_position.x,
            original_position.y - headSpeed * flameSpeedMove / 100,
            original_position.z
        );
        Renderer.transform.localPosition = new_position;

        UpdateSmokeEmitters(flameStrength);
        UpdateSound(flameStrength);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void InitialiseSmokeEmitters()
    {
        smokeEmitters = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < smokeEmitters.Length; i++)
        {
            var emitter = smokeEmitters[i].emission;
            emitter.rateOverTime = 0f;
        }
    }

    // Update is called once per frame
    void UpdateSmokeEmitters(float flameStrength)
    {
        for (int i = 0; i < smokeEmitters.Length; i++)
        {
            var emitter = smokeEmitters[i].emission;
            emitter.rateOverTime = flameStrength * smokeEmissionScale;
        }
    }

    void UpdateSound(float flameStrength)
    {
        EngineAudio.volume = Mathf.Min(flameStrength, 50f) / 50f;
    }
}
