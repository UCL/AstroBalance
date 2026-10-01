using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    private void SetMusic(MusicCue cue)
    {
        MusicManager.getInstance().updateMusic(cue);
    }

    private void StopMusic()
    {
        MusicManager.getInstance().stopMusic();
    }

    private void ChangeScene(string scene, MusicCue? cue)
    {
        if (cue.HasValue)
        {
            SetMusic(cue.Value);
        }
        else
        {
            StopMusic();
        }
        SFXManager.getInstance().stopSound(); // stop any looping sounds
        SceneManager.LoadScene(scene);
    }

    public void LoadMenuScreen()
    {
        ChangeScene("Scenes/MenuScreen", MusicCue.menu);
    }

    public void LoadBadgesScreen()
    {
        ChangeScene("Scenes/BadgesScreen", null);
    }

    public void LoadStarCollector()
    {
        ChangeScene("Scenes/StarCollector/StarCollector", MusicCue.starCollector);
    }

    public void LoadStarCollectorInstructions()
    {
        ChangeScene("Scenes/StarCollector/InstructionsStarCollector", MusicCue.menu);
    }

    public void LoadStarCollectorDemo()
    {
        ChangeScene("Scenes/StarCollector/DemoStarCollector", null);
    }

    public void LoadRocketLaunch()
    {
        ChangeScene("Scenes/RocketLaunch/RocketLaunch", null);
    }

    public void LoadRocketLaunchInstructions()
    {
        ChangeScene("Scenes/RocketLaunch/InstructionsRocketLaunch", MusicCue.menu);
    }

    public void LoadRocketLaunchDemo()
    {
        ChangeScene("Scenes/RocketLaunch/DemoRocketLaunch", null);
    }

    public void LoadStarSeek()
    {
        ChangeScene("Scenes/StarSeek/StarSeek", null);
    }

    public void LoadStarSeekInstructions()
    {
        ChangeScene("Scenes/StarSeek/InstructionsStarSeek", MusicCue.menu);
    }

    public void LoadStarSeekDemo()
    {
        ChangeScene("Scenes/StarSeek/DemoStarSeek", null);
    }

    public void LoadStarMap()
    {
        ChangeScene("Scenes/StarMap/StarMap", null);
    }

    public void LoadStarMapInstructions()
    {
        ChangeScene("Scenes/StarMap/InstructionsStarMap", MusicCue.menu);
    }

    public void LoadStarMapDemo()
    {
        ChangeScene("Scenes/StarMap/DemoStarMap", null);
    }

    public void LoadSpaceWalking()
    {
        ChangeScene("Scenes/SpaceWalk/SpaceWalking", null);
    }

    public void LoadSpaceWalkingInstructions()
    {
        ChangeScene("Scenes/SpaceWalk/InstructionsSpaceWalk", MusicCue.menu);
    }

    public void LoadSpaceWalkingDemo()
    {
        ChangeScene("Scenes/SpaceWalk/DemoSpaceWalk", null);
    }

    public void LoadZeroGravity()
    {
        ChangeScene("Scenes/ZeroGravity/ZeroGravity", null);
    }

    public void LoadZeroGravityInstructions()
    {
        ChangeScene("Scenes/ZeroGravity/InstructionsZeroGravity", MusicCue.menu);
    }

    public void LoadZeroGravityDemo()
    {
        ChangeScene("Scenes/ZeroGravity/DemoZeroGravity", null);
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
