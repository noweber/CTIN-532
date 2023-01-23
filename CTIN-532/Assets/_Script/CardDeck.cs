using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardDeck : MonoBehaviour
{
    public TextMeshProUGUI DrawTimerText;

    private float secondsBetweenDraws;
    private float secondsSinceLastDraw;

    private void Awake()
    {
        secondsBetweenDraws = 5.0f;
        secondsSinceLastDraw = 0.0f;
    }


    // Update is called once per frame
    void Update()
    {
        secondsSinceLastDraw += Time.deltaTime;
        while(secondsSinceLastDraw >= secondsBetweenDraws)
        {
            secondsSinceLastDraw -= secondsBetweenDraws;
            DrawCard();
        }
    }

    private void FixedUpdate()
    {
        UpdateDrawTimerText();
    }

    /// <summary>
    /// This function draws the next card from the deck.
    /// Currently, this is just a random number.
    /// </summary>
    /// <returns>Returns the drawn card.</returns>
    public int DrawCard()
    {
        int drawnCard = Random.Range(0, 10);
        Debug.Log("Card Drawn: " + drawnCard);
        return drawnCard;
    }

    /// <summary>
    /// Updates the referenced text component to depict the time until the next card draw time.
    /// </summary>
    public void UpdateDrawTimerText()
    {
        if (DrawTimerText != null)
        {
            DrawTimerText.text = "Next Card: " + (int) Math.Round(secondsBetweenDraws - secondsSinceLastDraw);
        }
    }
}
