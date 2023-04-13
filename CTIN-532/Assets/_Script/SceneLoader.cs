using UnityEngine;
using UnityEngine.SceneManagement; // Required for SceneManager

public class SceneLoader : MonoBehaviour
{
    public string sceneName; // Name of the scene to load

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
