using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void LoadStarCollector()
    {
        Debug.Log("Pressed button.");
        SceneManager.LoadScene("Scenes/StarCollector");
    }

    public void LoadRocketLaunch()
    {
        Debug.Log("Pressed button.");
        SceneManager.LoadScene("Scenes/RocketLaunch");
    }

    public void LoadStarSeek()
    {
        Debug.Log("Pressed button.");
        SceneManager.LoadScene("Scenes/StarSeek");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
