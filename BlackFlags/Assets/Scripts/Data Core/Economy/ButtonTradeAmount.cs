using UnityEngine.UI;
using UnityEngine;
using GameMechanics.Data;

public class ButtonTradeAmount : MonoBehaviour
{
    [SerializeField] private int buttonAmount;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate { Action(); });
    }

    protected virtual void Action()
    {
        TradeManager.Amount = buttonAmount;
    }

    protected int GetAmount()
    {
        return buttonAmount;
    }
}
