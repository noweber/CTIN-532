using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictStateChangeHelpers : MonoBehaviour
    {
        /// <summary>
        /// A helper method used by UI buttons to navigate to the district start state.
        /// </summary>
        public void ChangeToDistrictStart()
        {
            DependencyService.Instance.DistrictFsm().ChangeState(DistrictState.Start);
        }
    }
}
