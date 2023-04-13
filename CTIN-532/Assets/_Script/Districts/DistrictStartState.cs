using Assets._Script.Game;
using System;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictStartState : MonoBehaviour, IGameState
    {
        public GameObject StartOfLevelUi;

        public Guid? SessionId = null;

        public void OnEnter()
        {
            if (SessionId == null)
            {
                SessionId = Guid.NewGuid();
            }
            StartOfLevelUi.SetActive(true);
            AudioManager.Instance.DistrictStartZinger.Play();
            DependencyService.Instance.DistrictController().CreateDistrict();
            Analytics.Instance.SendAnalyticsData(SessionId.Value.ToString(), DependencyService.Instance.DistrictController().DistrictNumber);
            DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Play);
        }

        public void OnExit()
        {
            StartOfLevelUi.SetActive(false);
        }
    }
}
