using UnityEngine;
using UnityEngine.UI;

public class UnitUiController : MonoBehaviour
{
    public AudioClip SpawnSound;
    public Image Character;

    public Image Number;

    public Image Jewel;

    public int type;

    public void OnDestroy()
    {
        AudioManager.Instance.PlaySFX(SpawnSound, 1.0f);
    }
}
