using Assets._Script.Game;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictPlayState : MonoBehaviour, IGameState
    {
        public List<GameObject> districtGameObjects;

        public void OnEnter()
        {
            AudioManager.Instance.DistrictMusic.Play();
            SetDistrictGameObjectsActiveState(true);
            CurrencyController.Instance.ResetData();
        }

        public void OnExit()
        {
            AudioManager.Instance.DistrictMusic.Stop();
            SetDistrictGameObjectsActiveState(false);
        }

        private void SetDistrictGameObjectsActiveState(bool state)
        {
            foreach (var districtObject in districtGameObjects)
            {
                districtObject.SetActive(state);
            }
        }
    }
}
