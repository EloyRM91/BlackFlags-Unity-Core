
namespace GameMechanics.Sound
{
    public class Click1Response : ResponsiveSound
    {
        protected override void Response()
        {
            CallResponse(Sound.AudioResponse.click1);
        }
    }
}

