using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardDeckController : MonoBehaviour
{
    public GameObject UnitCardPrefab;

    public List<Sprite> CardSprites;

    public List<GameObject> CardSlots;

    public TextMeshProUGUI NextCardDrawTimerText;

    public float SecondsBetweenCardDraws = 5.0f;

    private float secondsSinceLastCardDraw;

    private List<GameObject> unitCards;

    private bool cardSlotsFull;

    void Start()
    {
        cardSlotsFull = false;
        if (CardSlots != null)
        {
            unitCards = new List<GameObject>(new GameObject[CardSlots.Count]);
            Debug.Log("Unit cards lenght is: " + unitCards.Count);
        }
        secondsSinceLastCardDraw = SecondsBetweenCardDraws;
    }

    void FixedUpdate()
    {
        if (!cardSlotsFull)
        {
            secondsSinceLastCardDraw += Time.fixedDeltaTime;
            if (secondsSinceLastCardDraw >= SecondsBetweenCardDraws)
            {
                secondsSinceLastCardDraw -= SecondsBetweenCardDraws;
                DrawCard();
            }
            UpdateCardDrawTimer();
        }
        else
        {
            secondsSinceLastCardDraw = SecondsBetweenCardDraws;
        }
        UpdateCardDrawTimer();
    }

    public void DrawCard()
    {
        foreach (GameObject cardSlot in CardSlots)
        {
            if (cardSlot == null)
            {
                Debug.LogError("A card slot is null.");
                return;
            }
        }

        for (int i = 0; i < unitCards.Count && i < CardSlots.Count; i++)
        {
            if (unitCards[i] == null)
            {
                Debug.Log("A card slot is assigned.");
                unitCards[i] = Instantiate(UnitCardPrefab, CardSlots[i].transform.position, Quaternion.identity, CardSlots[i].transform);
                if (CardSprites != null && CardSprites.Count > 0)
                {
                    UnitUiController unitUiController = unitCards[i].GetComponent<UnitUiController>();
                    if (unitUiController != null)
                    {
                        Random.InitState(System.DateTime.Now.Millisecond);
                        int type = Random.Range(0, CardSprites.Count);
                        unitUiController.Character.sprite = CardSprites[type];
                        unitUiController.type= type;
                    }
                }
                return;
            }
        }

        foreach (GameObject card in unitCards)
        {
            if (card == null)
            {
                cardSlotsFull = false;
            }
        }
    }

    private void UpdateCardDrawTimer()
    {
        if (NextCardDrawTimerText == null)
        {
            Debug.LogError("Timer text is null.");
            return;
        }

        NextCardDrawTimerText.text = "Next In: " + ((int)Math.Round(SecondsBetweenCardDraws - secondsSinceLastCardDraw)).ToString();
    }
}
