
namespace GameMechanics.Sound
{
    public class Click2Response : ResponsiveSound
    {
        protected override void Response()
        {
            CallResponse(Sound.AudioResponse.click2);
        }
    }
}
