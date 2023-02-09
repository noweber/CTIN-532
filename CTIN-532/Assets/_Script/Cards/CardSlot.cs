using TMPro;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public TextMeshProUGUI cooldownText;

    [SerializeField]
    private float cooldownInSeconds = 1.0f;

    [SerializeField]
    private float cooldownSecondsRemaining;

    [SerializeField]
    private GameObject drawnCard;

    [SerializeField]
    private CardDeck cardDeck;

    public void SetCooldown(float seconds)
    {
        if (seconds < 0)
        {
            seconds = 0;
        }
        cooldownInSeconds = seconds;
        //TODO: Ask Jackie... cooldownSecondsRemaining = seconds; ??
    }

    private void Start()
    {
        cardDeck = CardDeck.Instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // TODO: When turn-based pausing is enabled, make this timer occur in the deck which can keep all timers in sync to prevent floating point error.

        // Cooldowns don't matter unless there is no card in this slot:
        if (drawnCard == null)
        {
            cooldownSecondsRemaining -= Time.fixedDeltaTime;
            if (cooldownSecondsRemaining <= 0)
            {
                Debug.Log("Draw card.");
                if (cardDeck != null)
                {
                    drawnCard = cardDeck.InstantiateNextCard(transform.position, transform, Quaternion.identity);
                }
                else
                {
                    Debug.LogError("Card deck is null, so no card can be drawn. Make sure the scene has a CardDeck instance.");
                }
                cooldownSecondsRemaining = cooldownInSeconds;
                cooldownText.gameObject.SetActive(false);
            }
            else
            {
                if (cooldownText != null)
                {
                    if (!cooldownText.IsActive())
                    {
                        cooldownText.gameObject.SetActive(true);
                    }
                    cooldownText.text = ((int)cooldownSecondsRemaining).ToString() + "s";
                }
            }
        }
    }
}
