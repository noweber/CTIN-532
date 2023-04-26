using UnityEngine;
using static PlayerSelection;

public class UnitLogicSelectionButton : MonoBehaviour
{
    [SerializeField] private AudioSource selectionSoundEffect;

    public UnitLogic UnitLogicForButton;

    public GameObject Highlight;

    public KeyCode Keybind = KeyCode.None;

    PlayerSelection playerSelection;

    public void SetUnitLogicOnClick()
    {
        PlaySoundEffect();
        playerSelection.SelectUnitLogic(UnitLogicForButton);
        TurnAllButtonHighlightsOff();
        SetThisButtonsHighlightActiveState(true);
    }

    private void Start()
    {
        playerSelection = FindObjectOfType<PlayerSelection>();
    }

    private void PlaySoundEffect()
    {
        if (selectionSoundEffect != null)
        {
            selectionSoundEffect.Play();
        }
    }

    private void TurnAllButtonHighlightsOff()
    {
        UnitLogicSelectionButton[] buttons = FindObjectsOfType<UnitLogicSelectionButton>();
        foreach (var button in buttons)
        {
            button.SetThisButtonsHighlightActiveState(false);
        }
    }

    private void SetThisButtonsHighlightActiveState(bool highlightOn)
    {
        Highlight.SetActive(highlightOn);
    }

    private void Update()
    {
        if (Keybind != KeyCode.None)
        {
            if (Input.GetKeyDown(Keybind))
            {
                SetUnitLogicOnClick();
            }
        }
    }
}
