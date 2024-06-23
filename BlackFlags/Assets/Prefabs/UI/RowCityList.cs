using UnityEngine;
using GameMechanics.WorldCities;

public class RowCityList : MonoBehaviour
{
    public MB_City thisCity;

    public void FocusOnThisCity()
    {
        MapCamera.FocusOnTarget(thisCity.transform);
        UIMap.ui.ClearPanels();
    }
}
