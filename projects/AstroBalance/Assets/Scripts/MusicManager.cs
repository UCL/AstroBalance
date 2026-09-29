using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MusicCue
{
    splashScreen,
    menu,
    winScreen,
    badges,
    rocketLaunch,
    starCollector,
    starMap,
    starSeek,
    spaceWalk,
    zeroGravity,
    tutorial,
}

[System.Serializable]
public class MusicProperties
{
    public AudioClip clip;
    public float relativeVolume = 1f;
}

[System.Serializable]
public class Pieces
{
    public MusicCue key;
    public MusicProperties properties;
}

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    public float volume = 1f;

    [SerializeField]
    private MusicCue defaultMusic;

    [SerializeField]
    List<Pieces> pieces;

    Dictionary<MusicCue, MusicProperties> musicMap;

    AudioSource source;

    private MusicCue currentCue = MusicCue.splashScreen;

    private static MusicManager instance;

    public static MusicManager getInstance()
    {
        if (instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("MusicManager");

            instance = Instantiate(prefab).GetComponent<MusicManager>();
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

        source = GetComponent<AudioSource>();
        musicMap = new();
        // Construct map
        foreach (var piece in pieces)
        {
            musicMap.Add(piece.key, piece.properties);
        }
    }

    public void updateMusic(MusicCue cue)
    {
        if (source.isPlaying && (currentCue == cue))
        {
            return;
        }
        source.Stop();
        source.clip = musicMap[cue].clip;
        source.volume = musicMap[cue].relativeVolume * volume;
        source.Play();
        currentCue = cue;
    }

    public void stopMusic()
    {
        source.Stop();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source.loop = true;
        source.clip = musicMap[defaultMusic].clip;
        source.volume = musicMap[defaultMusic].relativeVolume * volume;
        source.Play();

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() { }
}
