using UnityEngine;

namespace Assets._Script.Game
{
    public class GameDistrictState : MonoBehaviour, IGameState
    {
        public void OnEnter()
        {
            DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Start);
        }
        public void OnExit()
        {
        }
    }
}
