
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public enum UnitLogic
    {
        Attack,
        Defend,
        Intercept,
        Hunt,
        Random
    }

    public UnitLogic SelectedLogic;

    public void SelectUnitLogic(UnitLogic selection)
    {
        SelectedLogic = selection;
    }
}
