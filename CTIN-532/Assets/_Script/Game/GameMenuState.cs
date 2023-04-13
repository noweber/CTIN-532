using UnityEngine;

namespace Assets._Script.Game
{
    public class GameMenuState : MonoBehaviour, IGameState
    {
        public GameObject Menu;

        public void OnEnter()
        {
            Menu.SetActive(true);
            AudioManager.Instance.MainMenuMusic.Play();
        }

        public void OnExit()
        {
            AudioManager.Instance.MainMenuMusic.Stop();
            Menu.SetActive(false);
        }
    }
}
