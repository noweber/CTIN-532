using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardDeck : Singleton<CardDeck>
{
    public List<GameObject> CardPrefabs;

    public List<CardSlot> CardSlots;

    public float ReplacementCooldownInSeconds = 10.0f;

    private float remainingCardSlotCooldownInSeconds;

    private void Awake()
    {
        remainingCardSlotCooldownInSeconds = 0;
    }

    void Start()
    {
        UpdateCardSlotCooldowns(0);
    }

    void FixedUpdate()
    {
        if (IsACardSlotEmpty())
        {
            remainingCardSlotCooldownInSeconds -= Time.fixedDeltaTime;
            UpdateCardSlotCooldowns(remainingCardSlotCooldownInSeconds);

            if (remainingCardSlotCooldownInSeconds <= 0)
            {
                remainingCardSlotCooldownInSeconds = ReplacementCooldownInSeconds;
                DrawCardForEachEmptySlot();
                Time.timeScale = 0;
            }
        }
    }

    private bool IsACardSlotEmpty()
    {
        foreach (var slot in CardSlots)
        {
            if (slot == null)
            {
                Debug.LogError("Card slot not set on the game object.");
            }
            else if (slot.DrawnCard == null)
            {
                return true;
            }
        }
        return false;
    }

    private void DrawCardForEachEmptySlot()
    {
        foreach (var slot in CardSlots)
        {
            if (slot.DrawnCard == null)
            {
                slot.DrawnCard = InstantiateNextCard(slot.transform.position, slot.transform, Quaternion.identity);
            }
        }
    }

    public void UpdateCardSlotCooldowns(float remainingSeconds)
    {
        foreach (var slot in CardSlots)
        {
            if (slot != null)
            {
                slot.SetRemainingCooldownTimer(remainingSeconds);
            }
            else
            {
                Debug.LogError("A card slot is null. Ensure these references are set.");
            }
        }
    }

    public GameObject InstantiateNextCard(Vector3 postion, Transform parent, Quaternion rotation)
    {
        if (CardPrefabs == null || CardPrefabs.Count == 0)
        {
            throw new Exception("The set of card prefabs are notset in the inspector.");
        }

        foreach (GameObject unitPrefab in CardPrefabs)
        {
            if (unitPrefab == null)
            {
                throw new Exception("A unit prefab is null. Ensure these are set in the inspector.");
            }
        }

        return Instantiate(CardPrefabs[Random.Range(0, CardPrefabs.Count)], postion, rotation, parent);
    }

}
