using TMPro;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    public ResourceCountUiController CurrencyResource;

    public PlayerResourcesController playerResources;

    public const int CurrencyIncrement = 100;

    public int TotalCurrency { get; private set; }
    public int MaxCurrency { get; private set; }

    private float secondsBetweenIncrements;

    private float secondsElapsedSinceLastMoneyIncrement;

    // Start is called before the first frame update
    void Start()
    {
        this.TotalCurrency = 0;
        this.secondsBetweenIncrements = 1.0f;
        this.secondsElapsedSinceLastMoneyIncrement = 0;
    }

    private void FixedUpdate()
    {
        MaxCurrency = playerResources.GetMaxNumberOfUnitsThePlayerCanHave() * CostToDeployAUnit(); // TODO: Instead of 10, use a cost of units.
    }

    public bool CanAffordAnotherUnit()
    {
        return TotalCurrency >= CostToDeployAUnit();
    }

    private int CostToDeployAUnit()
    {
        return CurrencyIncrement * 10;
    }

    // Update is called once per frame
    void Update()
    {
        this.secondsElapsedSinceLastMoneyIncrement += Time.deltaTime;

        while (this.secondsElapsedSinceLastMoneyIncrement > this.secondsBetweenIncrements)
        {
            this.secondsElapsedSinceLastMoneyIncrement -= this.secondsBetweenIncrements;
            if (this.TotalCurrency < MaxCurrency)
            {
                this.TotalCurrency += CurrencyIncrement;
                this.UpdateMoneyText();
            }
        }
    }

    public void SpendMoney(int moneySpent)
    {
        this.TotalCurrency -= moneySpent;
        this.UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        if (CurrencyResource != null)
        {
            CurrencyResource.SetResourceCount(TotalCurrency);
        }
    }
}
