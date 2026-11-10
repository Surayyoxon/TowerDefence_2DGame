
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

    // Pul yetarliligini tekshirish
    public bool HasEnoughMoney(int amount)
    {
        return currentMoney >= amount;
    }

    // Tower sotib olish uchun pul ayirish
    public bool TrySpendMoney(int amount)
    {
        if (amount < 0 || !HasEnoughMoney(amount))
        {
            Debug.Log("Pul yetarli emas!");
            return false;
        }

        currentMoney -= amount;

        UpdateMoneyUI();

        Debug.Log("Money spent: " + amount);
        Debug.Log("Current Money: " + currentMoney);

        return true;
    }

    // Enemy o'ldirilganda pul qo'shish
    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        currentMoney += amount;

        UpdateMoneyUI();

        Debug.Log("Money earned: " + amount);
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
