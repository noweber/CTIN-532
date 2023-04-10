using Assets._Script.Game;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictPlayState : MonoBehaviour, IGameState
    {
        public List<GameObject> districtGameObjects;

        public void OnEnter()
        {
            Debug.Log(MethodBase.GetCurrentMethod());
            AudioManager.Instance.DistrictMusic.Play();
            SetDistrictGameObjectsActiveState(true);
            DependencyService.Instance.DistrictController().CreateDistrict(DependencyService.Instance.DistrictController().DistrictNumber);
        }

        public void OnExit()
        {
            Debug.Log(MethodBase.GetCurrentMethod());
            SetDistrictGameObjectsActiveState(false);
        }

        private void SetDistrictGameObjectsActiveState(bool state)
        {
            foreach(var districtObject in districtGameObjects)
            {
                districtObject.SetActive(state);
            }
        }
    }
}
