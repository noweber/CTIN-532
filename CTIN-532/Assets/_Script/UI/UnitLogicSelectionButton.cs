using UnityEngine;
using static PlayerSelection;

public class UnitLogicSelectionButton : MonoBehaviour
{
    public UnitLogic UnitLogicForButton;

    public GameObject Highlight;

    PlayerSelection playerSelection;

    GameManager gameManager;

    private void Start()
    {
        playerSelection = FindObjectOfType<PlayerSelection>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetUnitLogicOnClick()
    {
        if(!gameManager.logicSelect_enabled) { return; }
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
}
