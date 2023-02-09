using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // TODO: handle mouseover event

    public Sprite sprite;

    public float hitPoints;

    public float attack;

    public float speed;

    public string description;

    public Image UnitImageComponent;

    public TextMeshProUGUI HpTextComponent;

    public TextMeshProUGUI AttackTextComponent;

    public TextMeshProUGUI SpeedTextComponent;

    public TextMeshProUGUI InfoTextComponent;

    public GameObject UnitWorldSpacePrefab;

    // TODO: remove this when refactoring SelectCard() below!
    public int UnitType;

    private SelectedObjects playerSelection;

    private void Awake()
    {
        if (description == null)
        {
            description = string.Empty;
        }

        if (sprite != null && UnitImageComponent != null)
        {
            UnitImageComponent.sprite = sprite;
        }

        if (HpTextComponent != null)
        {
            HpTextComponent.text = "HP: " + hitPoints.ToString();
        }

        if (AttackTextComponent != null)
        {
            AttackTextComponent.text = "ATT: " + attack.ToString();
        }

        if (SpeedTextComponent != null)
        {
            SpeedTextComponent.text = "SPD: " + speed.ToString();
        }

        if (InfoTextComponent != null)
        {
            InfoTextComponent.text = "INFO: " + description.ToString();
        }

        // TODO: Check this code for errors with debug statements
        // TODO: Refactor this to make it re-usable by other classes.. probably has PSController or a game manager be able to provde the human controller.
        SelectedObjects[] controllers = FindObjectsOfType<SelectedObjects>();
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
            if (UnitWorldSpacePrefab != null)
            {
                // TODO: remove this hard-coding of unity type values and just push it to the unit prefabs instead of the player selection
                playerSelection.SelectUnitToSpawn(UnitWorldSpacePrefab, UnitType, hitPoints, attack, speed);
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

        // TODO: Refactor the way this interacts with audio manager to store the spawn clip on the base prefab instead.
        AudioManager.Instance.PlaySFX(AudioManager.Instance.SpawnSound.clip, 1.0f);
        Destroy(this.gameObject);
    }
}
