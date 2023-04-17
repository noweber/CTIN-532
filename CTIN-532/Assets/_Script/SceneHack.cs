using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHack : Singleton<SceneHack>
{
    private static bool startSceneLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!startSceneLoaded)
        {
            startSceneLoaded = true;
            SceneManager.LoadScene("MainMenuTest");
        }
    }
}
