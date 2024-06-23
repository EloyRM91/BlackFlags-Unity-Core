using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour
{
    private static Camera _mainCamera, _minimapCamera, _3DViewCamera, _full3DViewCamera;

    public static Camera MainCamera
    {
        get { return _mainCamera; }
    }

    public static Camera MinimapCamera
    {
        get { return _minimapCamera; }
    }

    //public static Camera ViewCamera3D
    //{
    //    get { return _3DViewCamera; }
    //}

    //public static Camera FullViewCamera3D
    //{
    //    get { return _full3DViewCamera; }
    //}
    void Awake()
    {
        _mainCamera = transform.GetChild(0).GetChild(0).GetComponent<Camera>();
        _minimapCamera = transform.GetChild(2).GetComponent<Camera>();
        _3DViewCamera = transform.GetChild(4).GetComponent<Camera>();
        _full3DViewCamera = transform.GetChild(5).GetComponent<Camera>();

        UI.WorldMap.BannerController.cam = _mainCamera;
        Utilities.UI.MinimapButton.minimapCam = transform.GetChild(1).GetComponent<Camera>();
    }

    public static void Full3DMode(bool val)
    {
        _mainCamera.gameObject.SetActive(!val);
        _minimapCamera.gameObject.SetActive(!val);
        _3DViewCamera.gameObject.SetActive(!val);
        _full3DViewCamera.gameObject.SetActive(val);
    } 
    public static void ScenicViewMode(bool val)
    {
        _minimapCamera.gameObject.SetActive(!val);
        _3DViewCamera.gameObject.SetActive(!val);
    }

}
