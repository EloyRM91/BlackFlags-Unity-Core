//Core
using UnityEngine;
//Mechanics
using GameMechanics.Sound;

namespace UI.WorldMap
{
    /// <summary>
    /// this class makes a convoy selectable
    /// </summary>
    public class ShipSelector : MonoBehaviour
    {
        private void OnMouseEnter()
        {
            if(PlayerExplorer.IsVisible(transform))
                CursorManager.SetCursor(2);
        }
        private void OnMouseExit()
        {
            CursorManager.SetCursor(0);
        }
        protected virtual void OnMouseDown()
        {
            if (PlayerExplorer.IsVisible(transform))
            {
                if(Time.timeScale != 0)
                {
                    Ambience.SelectionSails_ST();
                    UISelector.instance.SetTarget(transform);
                }
            }
                
        }
    }
}

