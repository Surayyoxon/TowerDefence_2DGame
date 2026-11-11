
using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    [Header("Economy Settings")]
    [SerializeField] private int startingMoney = 500;

    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;

    private int currentMoney;

    public int CurrentMoney => currentMoney;

    private void Start()
    {
        currentMoney = startingMoney;
        UpdateMoneyUI();
    }

    // Check Money
    public bool HasEnoughMoney(int amount)
    {
        return amount > 0 && currentMoney >= amount;
    }

    // Spend Money
    public bool TrySpendMoney(int amount)
    {
        if (!HasEnoughMoney(amount))
        {
            Debug.Log("Not Enough Money!");
            return false;
        }

        currentMoney -= amount;

        UpdateMoneyUI();

        Debug.Log("Money Spent: " + amount);
        Debug.Log("Current Money: " + currentMoney);

        return true;
    }

    // Earn Money
    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        currentMoney += amount;

        UpdateMoneyUI();

        Debug.Log("Money Earned: " + amount);
        Debug.Log("Current Money: " + currentMoney);
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString();
        }
    }
}
