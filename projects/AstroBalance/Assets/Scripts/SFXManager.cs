using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SoundFX
{
    buttonPress,
    congratulations,
    thruster,
    rocketLaunchCharging,
    rocketLaunchNumber,
    starCollection,
    glimmer,
    complete,
    starMapCorrectClick,
    starMapCorrectSequence,
    starMapIncorrect,
    countdownTimer,
    countdownComplete,
    getReady,
    threeTwoOne,
    gameSelect,
}

[System.Serializable]
public class SoundProperties
{
    public AudioClip clip;
    public float relativeVolume = 1f;
}

[System.Serializable]
public class SoundEffect
{
    public SoundFX key;
    public SoundProperties sound;
}

public class SFXManager : MonoBehaviour
{
    [SerializeField, Tooltip("Overall SFX volume level")]
    public float volume = 1f;

    [SerializeField]
    private List<SoundEffect> AudioClips;

    [SerializeField, Tooltip("List of sounds needed in this particular scene.")]
    private Dictionary<SoundFX, SoundProperties> SoundMap;

    AudioSource source;

    private static SFXManager instance;

    public static SFXManager getInstance()
    {
        if (instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("SFXManager");

            instance = Instantiate(prefab).GetComponent<SFXManager>();
        }

        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        SoundMap = new();
        foreach (var fx in AudioClips)
        {
            SoundMap.Add(fx.key, fx.sound);
        }
        source = GetComponent<AudioSource>();
    }

    public void playSound(SoundFX effect)
    {
        var sound = SoundMap[effect];
        source.PlayOneShot(sound.clip, volume * sound.relativeVolume);
    }

    public void loopSound(SoundFX effect)
    {
        var sound = SoundMap[effect];
        source.resource = sound.clip;
        source.loop = true;
        source.Play();
    }

    public void stopSound()
    {
        source.Stop();
        source.loop = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() { }
}
