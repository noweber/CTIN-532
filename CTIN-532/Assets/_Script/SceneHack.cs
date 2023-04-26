using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneHack : Singleton<SceneHack>
{
    private static bool startSceneLoaded = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (!startSceneLoaded)
        {
            SceneManager.LoadScene("MainMenuTest");
            startSceneLoaded = true;
        }
    }

    public static bool WasStartSceneLoaded()
    {
        return startSceneLoaded;
    }
}
