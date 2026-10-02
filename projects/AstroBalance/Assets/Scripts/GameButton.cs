using UnityEngine;

public class GameButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void playSound()
    {
        SFXManager.getInstance().playSound(SoundFX.gameSelect);
    }
}
