using TMPro;
using UnityEngine;

public class PlayerMoneyController : MonoBehaviour
{
    public TextMeshProUGUI MoneyText;

    public int TotalMoney { get; private set; }

    private float baseSecondsBetweenMoneyIncrements;

    private float secondsUntilNextMoneyIncrement;

    private float secondsElapsedSinceLastMoneyIncrement;

    // Start is called before the first frame update
    void Start()
    {
        this.TotalMoney = 0;
        this.baseSecondsBetweenMoneyIncrements = 1.0f;
        this.secondsElapsedSinceLastMoneyIncrement = 0;
        this.SetTimeInSecondsUntilNextMoneyIncrement();
    }

    // Update is called once per frame
    void Update()
    {
        this.secondsElapsedSinceLastMoneyIncrement += Time.deltaTime;

        while(this.secondsElapsedSinceLastMoneyIncrement > this.secondsUntilNextMoneyIncrement)
        {
            this.secondsElapsedSinceLastMoneyIncrement -= this.secondsUntilNextMoneyIncrement;
            this.SetTimeInSecondsUntilNextMoneyIncrement();
            if (this.TotalMoney < 1000)
            {
                this.TotalMoney += 100;
                this.UpdateMoneyText();
            }
        }
    }

    public void SpendMoney(int moneySpent)
    {
        this.TotalMoney -= moneySpent;
        this.UpdateMoneyText();
        this.SetTimeInSecondsUntilNextMoneyIncrement();
    }

    private void UpdateMoneyText()
    {
        if (this.MoneyText != null)
        {
            this.MoneyText.text = "$ " + this.TotalMoney.ToString();
        }
    }

    private void SetTimeInSecondsUntilNextMoneyIncrement()
    {
        this.secondsUntilNextMoneyIncrement = ((this.TotalMoney / 100) + 1) * this.baseSecondsBetweenMoneyIncrements;
    }
}
