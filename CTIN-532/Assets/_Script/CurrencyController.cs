using Assets._Script;
using UnityEngine;

public class CurrencyController : Singleton<CurrencyController>
{
    public ResourceCountUiController CurrencyResource;

    public PlayerResourcesController playerResources;

    public const int CurrencyIncrement = 100;

    public int TotalCurrency { get; private set; }

    public int MaxCurrency { get; private set; }

    private float secondsBetweenIncrements;

    private float secondsElapsedSinceLastMoneyIncrement;

    void Start()
    {
        ResetData();
    }

    public void ResetData()
    {
        this.TotalCurrency = CostToDeployAUnit();
        this.secondsBetweenIncrements = 0.5f;
        this.secondsElapsedSinceLastMoneyIncrement = 0;
    }

    private void FixedUpdate()
    {
        MaxCurrency = playerResources.GetMaxNumberOfUnitsThePlayerCanHave() * CostToDeployAUnit();
    }

    public bool CanAffordAnotherUnit()
    {
        return TotalCurrency >= CostToDeployAUnit();
    }

    private int CostToDeployAUnit()
    {
        return 1000;
    }

    // Update is called once per frame
    void Update()
    {
        if (DependencyService.Instance.DistrictFsm().CurrentState != DistrictState.Play)
        {
            return;
        }

        this.secondsElapsedSinceLastMoneyIncrement += Time.deltaTime;

        while (this.secondsElapsedSinceLastMoneyIncrement > this.secondsBetweenIncrements)
        {
            this.secondsElapsedSinceLastMoneyIncrement -= this.secondsBetweenIncrements;
            if (this.TotalCurrency < MaxCurrency)
            {
                this.TotalCurrency += (int)(CurrencyIncrement * Mathf.Pow(playerResources.NodeCount, 0.5F));
                this.UpdateMoneyText();
            }
            if (this.TotalCurrency > MaxCurrency)
            {
                TotalCurrency = MaxCurrency;
            }
        }
    }

    public void PurchaseUnit()
    {
        this.TotalCurrency -= CostToDeployAUnit();
        this.UpdateMoneyText();
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
