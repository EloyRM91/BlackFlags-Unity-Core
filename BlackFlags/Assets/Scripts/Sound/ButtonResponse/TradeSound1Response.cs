
namespace GameMechanics.Sound
{
    public class TradeSound1Response : ResponsiveSound
    {
        protected override void Response()
        {
            CallResponse(Sound.AudioResponse.coin1);
        }
    }
}
