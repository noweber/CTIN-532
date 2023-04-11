using TMPro;
using UnityEngine;

namespace Assets._Script.SFX
{
    public class FloatingTextController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro floatingText;

        public void SetText(string text)
        {
            if (floatingText != null)
            {
                floatingText.text = text;
            }
        }
    }
}
