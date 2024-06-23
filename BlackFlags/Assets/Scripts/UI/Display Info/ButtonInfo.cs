using UnityEngine;

namespace UI.WorldMap
{
    [DisallowMultipleComponent]
    public class ButtonInfo : Info
    {
        [SerializeField] protected string txt;

        public override void DisplayInfo()
        {
            InfoDispatcher.DisplayInfo(txt);
        }
    }
}

