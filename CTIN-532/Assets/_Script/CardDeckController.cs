using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardDeckController : MonoBehaviour
{
    public GameObject UnitCardPrefab;

    public List<Sprite> CardSprites;

    public List<GameObject> CardSlots;

    private List<GameObject> unitCards;

    void Start()
    {
        if (CardSlots != null)
        {
            unitCards = new List<GameObject>(new GameObject[CardSlots.Count]);
            Debug.Log("Unit cards lenght is: " + unitCards.Count);
        }
    }

    void FixedUpdate()
    {
        foreach (GameObject card in unitCards)
        {
            if (card == null)
            {
                DrawCard();
            }
        }
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
                        unitUiController.Character.sprite = CardSprites[Random.Range(0, CardSprites.Count)];
                    }
                }
            }
        }
    }
}
