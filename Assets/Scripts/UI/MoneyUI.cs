using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private string moneySymbol = "$";

    private int lastMoney = int.MinValue;

    private void OnEnable()
    {
       MoneyState.MoneyChanged += HandleMoneyChanged;
       HandleMoneyChanged(MoneyState.Money);
    }

    private void OnDisable()
    {
        MoneyState.MoneyChanged -= HandleMoneyChanged;
    }

    private void HandleMoneyChanged(int currentMoney)
    {
        if(moneyText == null || currentMoney == lastMoney)
        {
            return;
        }

        lastMoney = currentMoney;
        moneyText.text = moneySymbol + currentMoney.ToString();      
    }
}
