using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField]
    public bool isPaused = false;

    public GameObject PauseMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = !isPaused;
            if (Time.timeScale == 0 && !isPaused)
            {
                Time.timeScale = 1;
            }
            else if (Time.timeScale == 1 && isPaused)
            {
                Time.timeScale = 0;
            }
        }

        if (Time.timeScale == 0)
        {
            isPaused = true;
            if (PauseMenu != null)
            {
                PauseMenu.SetActive(true);
            }
        }
        else if (Time.timeScale == 1)
        {
            isPaused = false;
            if (PauseMenu != null)
            {
                PauseMenu.SetActive(false);
            }
        }
    }
}
