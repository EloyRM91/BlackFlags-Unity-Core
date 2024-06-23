using UnityEngine;
using UnityEngine.UI;
using UI.WorldMap;

public class MinimapBannerController : BannerController
{
    private static Camera _minimapCam;
    void Start()
    {
        _minimapCam = Cameras.MinimapCamera;
    }

    protected override void SetUIPosition()
    {
        if (!UIMinimap.hiddenMap)
            transform.position = _minimapCam.WorldToScreenPoint(_target.position);

        if(GetComponent<Image>().enabled)
        {
            if (UIMinimap.hiddenMap)
                GetComponent<Image>().enabled = false;
        }
        else if(!UIMinimap.hiddenMap)
            GetComponent<Image>().enabled = true;
    }
}
