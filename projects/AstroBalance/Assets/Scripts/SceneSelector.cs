using UnityEngine;
using UnityEngine.SceneManagement;

// A static class to enable us to alternate between using pitch and yaw control
// on the rocket launch game.
public static class PitchOrYaw
{
    private static bool pitch = false;

    public static bool GetPitch()
    {
        pitch = !pitch;
	return pitch;
    }
}

public class SceneSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void LoadMenuScreen()
    {
        SceneManager.LoadScene("Scenes/MenuScreen");
    }

    public void LoadStarCollector()
    {
        SceneManager.LoadScene("Scenes/StarCollector");
    }

    public void LoadRocketLaunch()
    {
        SceneManager.LoadScene("Scenes/RocketLaunch");
    }

    public void LoadStarSeek()
    {
        SceneManager.LoadScene("Scenes/StarSeek");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
