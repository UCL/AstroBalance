using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField]
    private SoundFX soundEffect;

    public void playSound()
    {
        SFXManager.getInstance().playSound(soundEffect);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
