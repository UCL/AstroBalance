using UnityEngine;
using UnityEngine.SceneManagement;
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

    public void LoadStarMap()
    {
        SceneManager.LoadScene("Scenes/StarMap");
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
