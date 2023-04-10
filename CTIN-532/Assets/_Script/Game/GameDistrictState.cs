using UnityEngine;

namespace Assets._Script.Game
{
    public class GameDistrictState : MonoBehaviour, IGameState
    {
        public void OnEnter()
        {
            var gameManager = DependencyService.Instance.Game();
            gameManager.resetGame();
            gameManager.gameState = 200;
            DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Start);
        }
        public void OnExit()
        {
            // TODO: Make this transition to a DistrictStopState
        }
    }
}
