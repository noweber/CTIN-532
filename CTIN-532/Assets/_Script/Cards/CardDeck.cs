using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardDeck : Singleton<CardDeck>
{
    public List<GameObject> cardPrefabs;

    public List<CardSlot> cardSlots;

    public float cardSlotCooldownInSeconds = 5.0f;

    void Start()
    {
        SetCardSlotCooldowns(cardSlotCooldownInSeconds);
    }

    public void SetCardSlotCooldowns(float seconds)
    {
        if (seconds < 0)
        {
            seconds = 0;
        }
        cardSlotCooldownInSeconds = seconds;
        foreach (var slot in cardSlots)
        {
            if (slot != null)
            {
                slot.SetCooldown(seconds);
            }
            else
            {
                Debug.LogError("A card slot is null. Ensure these references are set.");
            }
        }
    }

    public GameObject InstantiateNextCard(Vector3 postion, Transform parent, Quaternion rotation)
    {
        if(cardPrefabs == null || cardPrefabs.Count == 0)
        {
            throw new Exception("The set of card prefabs are notset in the inspector.");
        }

        foreach (GameObject unitPrefab in cardPrefabs)
        {
            if (unitPrefab == null)
            {
                throw new Exception("A unit prefab is null. Ensure these are set in the inspector.");
            }
        }

        return Instantiate(cardPrefabs[Random.Range(0, cardPrefabs.Count)], postion, rotation, parent);
    }
}
