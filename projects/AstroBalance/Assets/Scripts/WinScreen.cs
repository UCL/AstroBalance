using UnityEngine;

public class WinScreen : MonoBehaviour
{
    SFXManager sfxManager;
    MusicManager musicManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sfxManager = SFXManager.getInstance();
        musicManager = MusicManager.getInstance();

        sfxManager.playSound(SoundFX.congratulations);
        musicManager.stopMusic();
    }

    // Update is called once per frame
    void Update() { }
}
