using TMPro;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public TextMeshProUGUI cooldownText;

    public GameObject DrawnCard;

    public GameObject CardSlotBackground;

    private void Awake()
    {
        DrawnCard = null;
    }

    public void SetRemainingCooldownTimer(float seconds)
    {
        if (DrawnCard != null) {
            Card cardMono = DrawnCard.GetComponent<Card>();
            if (cardMono != null)
            {
                if (seconds > 0)
                {
                    cardMono.AllowUse(false);
                    CardSlotBackground.SetActive(false);
                }
                else
                {
                    cardMono.AllowUse(true);
                    CardSlotBackground.SetActive(true);
                }
            } else
            {
                Debug.LogError("The game object in the card slot does not have a Card MonoBehavior entity.");
            }
        } else
        {
            CardSlotBackground.SetActive(false);
        }

        if (cooldownText != null)
        {
            if (seconds <= 0)
            {
                cooldownText.gameObject.SetActive(false);
            }
            else
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = ((int)seconds).ToString() + "s";
            }
        }
    }
}
