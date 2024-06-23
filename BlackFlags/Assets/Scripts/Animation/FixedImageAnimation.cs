using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldMap
{
    [RequireComponent(typeof(Image))]
    public class FixedImageAnimation : FixedTimeAnimation<Sprite>
    {
        protected override void Action(int n)
        {
            GetComponent<Image>().sprite = resources[n];
        }
    }
}

