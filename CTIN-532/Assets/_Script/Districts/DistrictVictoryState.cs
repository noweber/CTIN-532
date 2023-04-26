using Assets._Script.Game;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictVictoryState : DistrictBaseState, IGameState
    {

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
            DependencyService.Instance.DistrictController().NextDistrict();
        }
    }
}
