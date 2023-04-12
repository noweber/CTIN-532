using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.GridLayoutGroup;

public class TutorialMangaer : Singleton<TutorialMangaer>
{
    public GameObject StartUI;
    public GameObject HUD;
    public GameObject Tutorial;
    public GameObject endUI;
    public GameObject replayUI;

    public PopupWindow[] popUps;
    private int index = 0;

    public TextMeshProUGUI CountText;

    private void Update()
    {
        /*
        if(gameManager.gameState == 0)
        {
            StartUI.SetActive(true);
            HUD.SetActive(false);
            Tutorial.SetActive(false);
            endUI.SetActive(false);
        }

        if (gameManager.gameState == 300)
        {
            PlayerResourcesController p = PlayerResourcesManager.Instance.GetPlayerResourcesController(MapNodeController.Player.Human);
            StartUI.SetActive(false);
            HUD.SetActive(false);
            Tutorial.SetActive(false);
            endUI.SetActive(true);
        }

        if (gameManager.gameState == 100)
        {
            replayUI.SetActive(true);
        }
        else
        {
            replayUI.SetActive(false);
        }

        if (gameManager.gameState > 0 && gameManager.gameState<200)
        {
            if(index == popUps.Length)
            {
                popUps[index - 1].setActive(false);
            }
            else
            {
                if (index == 0)
                {
                    popUps[index].prevButton.SetActive(false);
                }

                if (popUps[index].canNext)
                {
                    showNext();
                }
                else if (popUps[index].canPrev)
                {
                    showPre();
                }
            }
        }*/
    }

    public void showNext()
    {
        popUps[index].setActive(false);
        index++;
        popUps[index].setActive(true);
    }

    public void showPre()
    {
        popUps[index].setActive(false);
        index--;
        popUps[index].setActive(true);
    }

    public void skipTutorial()
    {
       // gameManager.resetGame();
       // gameManager.gameState = 200;
        Tutorial.SetActive(false);
        PlayerResourcesController p = PlayerResourcesManager.Instance.GetPlayerResourcesController(MapNodeController.Player.Human);
    }

    public void showTutorial(bool s)
    {
        if (s)
        {
            //TODO seperate tutorial steps
            //gameManager.gameState = 1;
           /// gameManager.enableAll();
            //gameManager.resetGame();
            Debug.Log("show tutorial");
            index = 0;
            popUps[0].setActive(true);
            Tutorial.SetActive(true);
        }
        else
        {
            Tutorial.SetActive(false);
            StartUI.SetActive(false);
            //Debug.Log("load game");
           // gameManager.resetGame();
            //gameManager.gameState = 200;
        }
        //AudioManager.Instance.MainMenuMusic.Stop();
        //AudioManager.Instance.DistrictMusic.Play();
        StartUI.SetActive(false);
        HUD.SetActive(true);
        PlayerResourcesController p = PlayerResourcesManager.Instance.GetPlayerResourcesController(MapNodeController.Player.Human);
    }

    public void returnToMainmenu()
    {
        //gameManager.resetGame();
      //  gameManager.gameState = 0;
        PlayerResourcesController p = PlayerResourcesManager.Instance.GetPlayerResourcesController(MapNodeController.Player.Human);
    }
}
