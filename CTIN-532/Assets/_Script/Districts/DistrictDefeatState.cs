using Assets._Script.Game;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictDefeatState : MonoBehaviour, IGameState
    {
        [SerializeField]
        private GameObject defeatUi;

        public void OnEnter()
        {
            defeatUi.SetActive(true);
            //DependencyService.Instance.Game().resetGame();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Defeat.clip);
            // TODO: transition to main menu instead
            //TutorialMangaer.Instance.returnToMainmenu();
            //DependencyService.Instance.DistrictController().PreviousDistrict();
            //DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Start);
            //DependencyService.Instance.Game().gameState = 0;
        }

        public void OnExit()
        {
            defeatUi.SetActive(false);
        }
    }
}
