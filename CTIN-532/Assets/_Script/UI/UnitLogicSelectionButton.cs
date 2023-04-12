using UnityEngine;
using static PlayerSelection;

public class UnitLogicSelectionButton : MonoBehaviour
{
    public UnitLogic UnitLogicForButton;

    public GameObject Highlight;

    public KeyCode Keybind = KeyCode.None;

    PlayerSelection playerSelection;

    private void Start()
    {
        playerSelection = FindObjectOfType<PlayerSelection>();
    }

    public void SetUnitLogicOnClick()
    {
        playerSelection.SelectUnitLogic(UnitLogicForButton);
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

    private void Update()
    {
        if(Keybind != KeyCode.None)
        {
            if(Input.GetKeyDown(Keybind))
            {
                SetUnitLogicOnClick();
            }
        }
    }
}
