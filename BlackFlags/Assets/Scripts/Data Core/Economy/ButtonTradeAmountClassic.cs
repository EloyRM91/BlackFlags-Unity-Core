using GameMechanics.Data;

public class ButtonTradeAmountClassic : ButtonTradeAmount
{
    protected override void Action()
    {
        TradeManager.AmountClassic = GetAmount();
    }
}
