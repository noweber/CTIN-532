using Assets._Script.Game;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictStartState : MonoBehaviour, IGameState
    {
        public GameObject StartOfLevelUi;

        public void OnEnter()
        {
            StartOfLevelUi.SetActive(true);
            AudioManager.Instance.DistrictStartZinger.Play();
            DependencyService.Instance.DistrictController().CreateDistrict();
            DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Play);
        }

        public void OnExit()
        {
            StartOfLevelUi.SetActive(false);
        }
    }
}
