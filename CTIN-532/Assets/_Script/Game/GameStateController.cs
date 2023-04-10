using UnityEngine;

namespace Assets._Script.Game
{
    public class GameStateController : MonoBehaviour
    {
        /// <summary>
        /// A helper method used by UI buttons to easily navigate state changes.
        /// </summary>
        public void ChangeToDistrict()
        {
            DependencyService.Instance.GameFsm().ChangeState(GameState.District);
        }
    }
}
