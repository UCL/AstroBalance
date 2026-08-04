using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    public void LoadMenuScreen()
    {
        SceneManager.LoadScene("Scenes/MenuScreen");
    }

    public void LoadBadgesScreen()
    {
        SceneManager.LoadScene("Scenes/BadgesScreen");
    }

    public void LoadStarCollector()
    {
        SceneManager.LoadScene("Scenes/StarCollector/StarCollector");
    }

    public void LoadStarCollectorInstructions()
    {
        SceneManager.LoadScene("Scenes/StarCollector/InstructionsStarCollector");
    }

    public void LoadStarCollectorDemo()
    {
        SceneManager.LoadScene("Scenes/StarCollector/DemoStarCollector");
    }

    public void LoadRocketLaunch()
    {
        SceneManager.LoadScene("Scenes/RocketLaunch/RocketLaunch");
    }

    public void LoadRocketLaunchInstructions()
    {
        SceneManager.LoadScene("Scenes/RocketLaunch/InstructionsRocketLaunch");
    }

    public void LoadRocketLaunchDemo()
    {
        SceneManager.LoadScene("Scenes/RocketLaunch/DemoRocketLaunch");
    }

    public void LoadStarSeek()
    {
        SceneManager.LoadScene("Scenes/StarSeek");
    }

    public void LoadStarSeekInstructions()
    {
        SceneManager.LoadScene("Scenes/InstructionsStarSeek");
    }

    public void LoadStarMap()
    {
        SceneManager.LoadScene("Scenes/StarMap/StarMap");
    }

    public void LoadStarMapInstructions()
    {
        SceneManager.LoadScene("Scenes/StarMap/InstructionsStarMap");
    }

    public void LoadStarMapDemo()
    {
        SceneManager.LoadScene("Scenes/StarMap/DemoStarMap");
    }

    public void LoadSpaceWalking()
    {
        SceneManager.LoadScene("Scenes/SpaceWalking");
    }

    public void LoadSpaceWalkingInstructions()
    {
        SceneManager.LoadScene("Scenes/InstructionsSpaceWalk");
    }

    public void LoadZeroGravity()
    {
        SceneManager.LoadScene("Scenes/ZeroGravity");
    }

    public void LoadZeroGravityInstructions()
    {
        SceneManager.LoadScene("Scenes/InstructionsZeroGravity");
    }

    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update() { }
}
