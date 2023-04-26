using Assets._Script;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public enum UnitLogic
    {
        Attack,
        Defend,
        Intercept,
        Hunt,
        Split
    }

    public UnitLogic SelectedLogic;

    private void Start()
    {
        SelectUnitLogic(UnitLogic.Split);
    }

    public void SelectUnitLogic(UnitLogic selection)
    {
        SelectedLogic = selection;
        var units = DependencyService.Instance.DistrictController().GetUnitsByPlayer(true);
        foreach (var unit in units)
        {
            unit.ChangeLogic(selection);
        }
    }
}
