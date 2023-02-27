using UnityEngine;
using static PlayerSelection;

public class UnitLogicSelectionButton : MonoBehaviour
{
    public UnitLogic UnitLogicForButton;

    public GameObject Highlight;

    public void SetUnitLogicOnClick()
    {
        PlayerSelection.Instance.SelectUnitLogic(UnitLogicForButton);
        UnitLogicSelectionButton[] buttons = Object.FindObjectsOfType<UnitLogicSelectionButton>();
        foreach (var button in buttons)
        {
            button.FlagSelectionHighlight(false);
        }
        this.FlagSelectionHighlight(true);
    }

    public void FlagSelectionHighlight(bool highlightOn)
    {
        Highlight.SetActive(highlightOn);
    }
}
