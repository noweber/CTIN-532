using UnityEngine;
using UnityEngine.UI;

public class UnitUiController : MonoBehaviour
{
    public Image Character;

    public Image Number;

    public Image Jewel;

    public int type;

    public void OnDestroy()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.SpawnSound.clip, 1.0f);
    }
}
