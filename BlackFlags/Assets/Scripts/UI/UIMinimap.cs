//Unity
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//UI
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Mechanics
using GameMechanics.Utilities;

//Tweening
using DG.Tweening;

public class UIMinimap : SingletonBehaviour<UIMinimap>, UIDetectable, IPointerEnterHandler, IPointerExitHandler
{
    //Singleton
    //public static UIMinimap map;

    //Graphic Raycasting
    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;

    //Game Camera
    [SerializeField] private Transform _worldCamPivot;
    [SerializeField] private Camera _mapCam;

    //Minimap Positioning Camera
    [SerializeField] private GameObject _backgroundCam;

    //Raycast
    private Ray _clickRay;
    private RaycastHit _clickHit;

    //Tweening Animation
    private bool minimized = false;
    [SerializeField] private Transform[] movableElements;
    private float[] offsetY;

    //Tweening calls
    public delegate void Enable(bool masked);
    public static Enable EnableMask;
    public static bool hiddenMap = false;

    private void Start()
    {
        //map = this;
        //Graphic Raycast
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();

        //Click events
        UIMap.userclick += UserClick;
        UIMap.uiTransition += React;
        UIMap.clearPanels += SliceUp;
        GameMechanics.WorldCities.KeyPoint.KPSelected += SliceDown;
        //EnableMask += IsHidden;

        //Closed panels Events
        UI_SpecialViewsHUD.EndOfView += BackgroundCam;

        offsetY = new float[movableElements.Length];
        for (int i = 0; i < movableElements.Length; i++)
        {
            offsetY[i] = movableElements[i].position.y;
        }
    }

    private void OnDestroy()
    {
        UIMap.userclick -= UserClick;
        UIMap.uiTransition -= React;
        UIMap.clearPanels -= SliceUp;
        GameMechanics.WorldCities.KeyPoint.KPSelected -= SliceDown;
        GameMechanics.WorldCities.KeyPoint.KPSelected -= SliceDown;
        UI_SpecialViewsHUD.EndOfView -= BackgroundCam;
        //EnableMask += IsHidden;
    }

    //Interface requests
#region INTERFACES
    public void OnPointerEnter(PointerEventData @event)
    {
        CursorManager.SetCursor(3);
    }
    public void OnPointerExit(PointerEventData @event)
    {
        CursorManager.SetCursor(0);
    }
    public static bool GetGraphicRaycastResultST() { return instance.GetGraphicRaycastResult(); }

    public bool GetGraphicRaycastResult()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        //Does the raycast hit the GUI minimap panel?
        if (results.Count != 0)
        {
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("MinimapUIBox"))
                {
                    return true;
                }
            }
        }
        return false;
    }
#endregion


    private void UserClick()
    {
        if (GetGraphicRaycastResult()) MoveCamera();
    }

    private void MoveCamera()
    {
        _clickRay.origin = _mapCam.ScreenToWorldPoint(Input.mousePosition);
        _clickRay.direction = Vector3.up * -1;

        if (Physics.Raycast(_clickRay, out _clickHit, 50))
        {
            _worldCamPivot.position = new Vector3(_clickHit.point.x, _worldCamPivot.position.y, _clickHit.point.z);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            SliceDown();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SliceUp();
        }
    }

    public static void SliceDownST()
    {
        instance.SliceDown();
    }

    private void SliceDown()
    {
        if(!minimized)
        {
            minimized = true;
            
            for (int i = 0; i < movableElements.Length; i++)
            {
                movableElements[i].DOMoveY(offsetY[i] - 420, 0.3f).SetUpdate(true);
            }
            BackgroundCam(false, 0.3f);
        }
    }

    public static void SliceUpST()
    {
        instance.SliceUp();
    }

    private void SliceUp()
    {
        if(minimized)
        {
            minimized = false;

            for (int i = 0; i < movableElements.Length; i++)
            {
                movableElements[i].DOMoveY(offsetY[i], 0.3f).SetUpdate(true);
            }
            EnableMask(true);
            transform.GetChild(0).gameObject.SetActive(true);
            BackgroundCam(true, 0.3f);
        }
    }

    private void React(bool entry)
    {
        if (entry)
            SliceDown();
        else
            SliceUp();
    }

    private void BackgroundCam(bool state, float time)
    {
        StopAllCoroutines();
        StartCoroutine(SetBGCam(state, time));
    }

    private IEnumerator SetBGCam(bool state, float timing)
    {
        
        transform.GetChild(0).gameObject.SetActive(state);

        yield return new WaitForSecondsRealtime(state ? timing: 0);
        _backgroundCam.SetActive(state);
        IsHidden(state);
        EnableMask(state);
    }

    private void IsHidden(bool masked)
    {
        hiddenMap = !masked;
    }
    
}
