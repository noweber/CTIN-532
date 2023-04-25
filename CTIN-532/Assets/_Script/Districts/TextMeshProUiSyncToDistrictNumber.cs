using TMPro;
using UnityEngine;

namespace Assets._Script.Districts
{
    public sealed class TextMeshProUiSyncToDistrictNumber : MonoBehaviour
    {
        private TextMeshProUGUI uiText;

        private DistrictController districtController;

        private void Start()
        {
            uiText = GetComponent<TextMeshProUGUI>();
            districtController = DependencyService.Instance.DistrictController();
        }

        private void LateUpdate()
        {
            if(uiText != null && districtController != null)
            {
                uiText.text = districtController.DistrictNumber.ToString();
            }
        }
    }
}
