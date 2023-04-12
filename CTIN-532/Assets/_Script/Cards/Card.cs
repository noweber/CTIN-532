using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    // TODO: handle mouseover event

    public Sprite sprite;

    public string unitName;

    public float hitPoints;

    public float damage;

    public float speed;

    public string description;

    public Image UnitImageComponent;

    public GameObject UnitImageBackground;

    public GameObject CardBackground;

    public TextMeshProUGUI NameTextComponent;

    public TextMeshProUGUI HpTextComponent;

    public TextMeshProUGUI DamageTextComponent;

    public TextMeshProUGUI SpeedTextComponent;

    public TextMeshProUGUI DescriptionTextComponent;

    public GameObject UnitWorldSpacePrefab;

    // TODO: remove this when refactoring SelectCard() below!
    public int UnitType;

    private SelectedObjects playerSelection;

    private bool isCardSelectable;

    //private GameManager gameManager;

    private void Awake()
    {
        if (unitName == null)
        {
            unitName = string.Empty;
        }

        if (sprite != null && UnitImageComponent != null)
        {
            UnitImageComponent.sprite = sprite;
        }

        if (NameTextComponent != null)
        {
            NameTextComponent.text = "Name: " + unitName.ToString();
        }

        if (HpTextComponent != null)
        {
            HpTextComponent.text = "Hit Points: " + hitPoints.ToString();
        }

        if (DamageTextComponent != null)
        {
            DamageTextComponent.text = "Damage: " + damage.ToString();
        }

        if (SpeedTextComponent != null)
        {
            SpeedTextComponent.text = "Speed: " + speed.ToString();
        }

        if (DescriptionTextComponent != null)
        {
            DescriptionTextComponent.text = description;
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

        isCardSelectable = true;

        //gameManager = FindObjectOfType<GameManager>();
    }

    public void SelectCard()
    {
        //if (!gameManager.cardSelect_enabled) { return; }
        // game state

        if (!isCardSelectable)
        {
            return;
        }

        Time.timeScale = 1;

        if (playerSelection != null)
        {
            if (UnitWorldSpacePrefab != null)
            {
                // TODO: remove this hard-coding of unity type values and just push it to the unit prefabs instead of the player selection
                playerSelection.SelectUnitToSpawn(UnitWorldSpacePrefab, UnitType, hitPoints, damage, speed);
                playerSelection.SpawnUnit(UnitWorldSpacePrefab);
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
        //Destroy(this.gameObject);
    }

    public void AllowUse(bool state)
    {
        isCardSelectable = state;
        if (!isCardSelectable)
        {
            if (UnitImageBackground != null)
            {
                UnitImageBackground.SetActive(false);
            }

            if (CardBackground != null)
            {
                CardBackground.SetActive(false);
            }

            if (NameTextComponent != null)
            {
                NameTextComponent.color = new Color(NameTextComponent.color.r, NameTextComponent.color.g, NameTextComponent.color.b, 0);
            }

            if (HpTextComponent != null)
            {
                HpTextComponent.color = new Color(HpTextComponent.color.r, HpTextComponent.color.g, HpTextComponent.color.b, 0);
            }

            if (DamageTextComponent != null)
            {
                DamageTextComponent.color = new Color(DamageTextComponent.color.r, DamageTextComponent.color.g, DamageTextComponent.color.b, 0);
            }

            if (SpeedTextComponent != null)
            {
                SpeedTextComponent.color = new Color(SpeedTextComponent.color.r, SpeedTextComponent.color.g, SpeedTextComponent.color.b, 0);
            }
        }
        else
        {
            if (UnitImageBackground != null)
            {
                UnitImageBackground.SetActive(true);
            }

            if (CardBackground != null)
            {
                CardBackground.SetActive(true);
            }

            if (NameTextComponent != null)
            {
                NameTextComponent.color = new Color(NameTextComponent.color.r, NameTextComponent.color.g, NameTextComponent.color.b, 255);
            }

            if (HpTextComponent != null)
            {
                HpTextComponent.color = new Color(HpTextComponent.color.r, HpTextComponent.color.g, HpTextComponent.color.b, 255);
            }

            if (DamageTextComponent != null)
            {
                DamageTextComponent.color = new Color(DamageTextComponent.color.r, DamageTextComponent.color.g, DamageTextComponent.color.b, 255);
            }

            if (SpeedTextComponent != null)
            {
                SpeedTextComponent.color = new Color(SpeedTextComponent.color.r, SpeedTextComponent.color.g, SpeedTextComponent.color.b, 255);
            }
        }
    }
}
