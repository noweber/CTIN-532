using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMangaer : Singleton<TutorialMangaer>
{
    public GameObject StartUI;
    public GameObject HUD;
    public GameObject Tutorial;

    public PopupWindow[] popUps;
    private int index = 0;

    private GameManager gameManager;
    private LevelMono level;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        level = FindObjectOfType<LevelMono>();
    }
    private void Update()
    {
        if(gameManager.gameState == 1)
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
        }
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
        gameManager.resetGame();
        gameManager.gameState = 200;
        Tutorial.SetActive(false);
    }

    public void showTutorial(bool s)
    {
        if (s)
        {
            //TODO seperate tutorial steps
            gameManager.gameState = 1;
            gameManager.enableAll();
            Debug.Log("show tutorial");
            Tutorial.SetActive(true);
        }
        else
        {
            StartUI.SetActive(false);
            Debug.Log("load game");
            gameManager.gameState = 200;
        }
        AudioManager.Instance.MainMenuMusic.Stop();
        AudioManager.Instance.DistrictMusic.Play();
        StartUI.SetActive(false);
        HUD.SetActive(true);
    }

}
