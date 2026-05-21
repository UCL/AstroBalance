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
        SceneManager.LoadScene("Scenes/StarCollector");
    }

    public void LoadStarCollectorInstructions()
    {
        SceneManager.LoadScene("Scenes/InstructionsStarCollector");
    }

    public void LoadRocketLaunch()
    {
        SceneManager.LoadScene("Scenes/RocketLaunch");
    }

    public void LoadRocketLaunchInstructions()
    {
        SceneManager.LoadScene("Scenes/InstructionsRocketLaunch");
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
        SceneManager.LoadScene("Scenes/StarMap");
    }

    public void LoadStarMapInstructions()
    {
        SceneManager.LoadScene("Scenes/InstructionsStarMap");
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
