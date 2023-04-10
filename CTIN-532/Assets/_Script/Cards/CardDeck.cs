using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public List<GameObject> CardPrefabs;

    public List<CardSlot> CardSlots;

    public float ReplacementCooldownInSeconds = 10.0f;

    private float remainingCardSlotCooldownInSeconds;

    GameManager gameManager;
    public GameObject EnenemyStat;

    private void Awake()
    {
        remainingCardSlotCooldownInSeconds = 0;
        if (CardPrefabs.Count < CardSlots.Count)
        {
            Debug.LogError("There are not enough card prefabs to fill each slot.");
        }
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        UpdateCardSlotCooldowns(0);
    }

    void FixedUpdate()
    {
        if (gameManager.card_reset) {
            remainingCardSlotCooldownInSeconds = 0;
            gameManager.card_reset = false;
        }

        if(!gameManager.cardSelect_enabled) { return; }
        if (IsACardSlotEmpty())
        {
            remainingCardSlotCooldownInSeconds -= Time.fixedDeltaTime;
            UpdateCardSlotCooldowns(remainingCardSlotCooldownInSeconds);

            if (remainingCardSlotCooldownInSeconds <= 0)
            {
                remainingCardSlotCooldownInSeconds = ReplacementCooldownInSeconds;
                DrawCardForEachEmptySlot();
                //Time.timeScale = 0;
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
        for (int i = 0; i < CardSlots.Count; i++)
        {
            if (CardSlots[i].DrawnCard == null)
            {
                CardSlots[i].DrawnCard = InstantiateNextCard(CardSlots[i].transform.position, CardSlots[i].transform, Quaternion.identity, i);
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

    public GameObject InstantiateNextCard(Vector3 postion, Transform parent, Quaternion rotation, int slotIndex)
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

        return Instantiate(CardPrefabs[slotIndex], postion, rotation, parent);
    }

    public void checkEnermy()
    {
        if(gameManager.cardSelect_enabled)
        {
            EnenemyStat.SetActive(!EnenemyStat.activeSelf);
        }
    }
}
