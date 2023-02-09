using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField]
    public bool isPaused = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
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
    }
}
