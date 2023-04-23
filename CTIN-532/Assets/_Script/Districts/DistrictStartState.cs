using Assets._Script.Game;
using System;
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
            Analytics.Instance.SendDistrictAnalytics(DependencyService.Instance.DistrictController().DistrictNumber);
            DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Play);
            AudioManager.Instance.DistrictMusic.Play();
        }

        public void OnExit()
        {
            StartOfLevelUi.SetActive(false);
        }
    }
}
