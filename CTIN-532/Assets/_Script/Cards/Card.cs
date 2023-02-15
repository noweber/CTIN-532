using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // TODO: handle mouseover event

    public Sprite sprite;

    public string name;

    public float hitPoints;

    public float attack;

    public float magic;

    public float armor;

    public float resist;

    public float speed;

    public Image UnitImageComponent;

    public TextMeshProUGUI NameTextComponent;

    public TextMeshProUGUI HpTextComponent;

    public TextMeshProUGUI AttackTextComponent;

    public TextMeshProUGUI MagicTextComponent;

    public TextMeshProUGUI ArmorTextComponent;

    public TextMeshProUGUI ResistTextComponent;

    public TextMeshProUGUI SpeedTextComponent;

    public GameObject UnitWorldSpacePrefab;

    // TODO: remove this when refactoring SelectCard() below!
    public int UnitType;

    private SelectedObjects playerSelection;

    private void Awake()
    {
        if (name == null)
        {
            name = string.Empty;
        }

        if (sprite != null && UnitImageComponent != null)
        {
            UnitImageComponent.sprite = sprite;
        }

        if (NameTextComponent != null)
        {
            NameTextComponent.text = "Name: " + name.ToString();
        }

        if (HpTextComponent != null)
        {
            HpTextComponent.text = "Hit Points: " + hitPoints.ToString();
        }

        if (AttackTextComponent != null)
        {
            AttackTextComponent.text = "Attack: " + attack.ToString();
        }

        if (MagicTextComponent != null)
        {
            MagicTextComponent.text = "Magic: " + magic.ToString();
        }

        if (ArmorTextComponent != null)
        {
            ArmorTextComponent.text = "Armor: " + armor.ToString();
        }

        if (ResistTextComponent != null)
        {
            ResistTextComponent.text = "Resist: " + magic.ToString();
        }

        if (SpeedTextComponent != null)
        {
            SpeedTextComponent.text = "Speed: " + speed.ToString();
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
                playerSelection.SelectUnitToSpawn(UnitWorldSpacePrefab, UnitType, hitPoints, attack, magic, armor, resist, speed);
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
