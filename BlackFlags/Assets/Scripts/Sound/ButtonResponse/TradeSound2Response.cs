using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.Sound
{
    public class TradeSound2Response : ResponsiveSound
    {
        protected override void Response()
        {
            CallResponse(Sound.AudioResponse.coin2);
        }
    }
}
