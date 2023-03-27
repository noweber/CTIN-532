using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{

    LevelMono level;
    GameManager gameManager;

    private void Start()
    {
        level = FindObjectOfType<LevelMono>();
        gameManager = FindObjectOfType<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }*/

        // new game
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.resetGame();
            DistrictMetricsTelemetryManager.Instance.ResetDistrict();
        }
    }
}
