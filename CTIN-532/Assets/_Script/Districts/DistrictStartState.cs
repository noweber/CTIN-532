using Assets._Script.Game;
using System.Reflection;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictStartState : MonoBehaviour, IGameState
    {
        public GameObject StartOfLevelUi;

        public float SecondsUntilStateTransition = 5.0f;

        private float secondsUntilPlayState;

        private bool shouldUpdate;

        public void OnEnter()
        {
            StartOfLevelUi.SetActive(true);
            secondsUntilPlayState = SecondsUntilStateTransition;
            shouldUpdate = true;
            AudioManager.Instance.DistrictStartZinger.Play();
            // TODO: create a new map
        }

        public void OnExit()
        {
            StartOfLevelUi.SetActive(false);
        }

        void Update()
        {
            if (shouldUpdate)
            {
                secondsUntilPlayState -= Time.deltaTime;
                if (secondsUntilPlayState < 0)
                {
                    shouldUpdate = false;
                    DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Play);
                }
            }
        }
    }
}
