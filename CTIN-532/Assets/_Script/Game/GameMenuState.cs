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
            Menu.SetActive(false);
        }
    }
}
