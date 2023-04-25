using Assets._Script.Game;
using TMPro;
using UnityEngine;

namespace Assets._Script.Districts
{
    public class DistrictDefeatState : MonoBehaviour, IGameState
    {
        [SerializeField]
        private GameObject defeatUi;

        [SerializeField]
        private TextMeshProUGUI scoreText;

        public void OnEnter()
        {
            defeatUi.SetActive(true);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.Defeat.clip);
            if (scoreText != null)
            {
                scoreText.text = (DependencyService.Instance.DistrictController().DistrictNumber * 1000).ToString();
            }
            DependencyService.Instance.DistrictController().PreviousDistrict();
        }

        public void OnExit()
        {
            defeatUi.SetActive(false);
        }
    }
}
