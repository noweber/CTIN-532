using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHack : MonoBehaviour
{
    private static bool startSceneLoaded = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!startSceneLoaded)
        {
            startSceneLoaded = true;
            SceneManager.LoadScene("MainMenuTest");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
