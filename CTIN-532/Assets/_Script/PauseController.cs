using Assets._Script;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField]
    public bool isPaused = false;

    public GameObject PauseMenu;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(DependencyService.Instance.DistrictFsm().CurrentState != DistrictState.Play)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))// && gameManager.cardSelect_enabled)
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
