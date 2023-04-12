using Assets._Script.Game;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictVictoryState : MonoBehaviour, IGameState
    {
        public void OnEnter()
        {
            //DependencyService.Instance.Game().resetGame();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Victory.clip);
            DependencyService.Instance.DistrictController().NextDistrict();
            DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Start);
        }

        public void OnExit()
        {
        }
    }
}
