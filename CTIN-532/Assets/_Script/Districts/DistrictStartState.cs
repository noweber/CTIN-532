using Assets._Script.Game;
using System;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictStartState : MonoBehaviour, IGameState
    {
        public GameObject StartOfLevelUi;

        public Guid SessionId;

        void Awake()
        {
            SessionId = Guid.NewGuid();
        }

        public void OnEnter()
        {
            StartOfLevelUi.SetActive(true);
            AudioManager.Instance.DistrictStartZinger.Play();
            DependencyService.Instance.DistrictController().CreateDistrict();
            Analytics.Instance.SendAnalyticsData(SessionId.ToString(), DependencyService.Instance.DistrictController().DistrictNumber);
            DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Play);
        }

        public void OnExit()
        {
            StartOfLevelUi.SetActive(false);
        }
    }
}
