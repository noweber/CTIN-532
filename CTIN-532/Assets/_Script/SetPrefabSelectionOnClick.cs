using UnityEngine;

public class SetPrefabSelectionOnClick : MonoBehaviour
{
    public GameObject UnitPrefab;

    public GameObject UiRootElementForDeletion;

    private PlayerSelectionController playerSelection;

    public void Start()
    {
        PlayerSelectionController[] controllers = FindObjectsOfType<PlayerSelectionController>();
        foreach (var controller in controllers)
        {
            if (controller.Owner == MapNodeController.Player.Human)
            {
                playerSelection = controller;
            }
        }
    }

    public void SelectCard()
    {
        if (playerSelection != null)
        {
            if (UnitPrefab != null)
            {
                playerSelection.SelectedUnitPrefab = UnitPrefab;
                playerSelection.SelectedUnitSprite = UiRootElementForDeletion.GetComponent<UnitUiController>().Character.sprite;
            }
            else
            {
                Debug.LogError("The unit prefab is null when its card is selected from the UI to spawn.");
            }
        }
        else
        {
            Debug.LogError("The player selection controller was null and the selected card from the UI cannot be assigned.");
        }

        // TODO: Fix this so the scripts are on the same level.
        Destroy(UiRootElementForDeletion);
    }
}
