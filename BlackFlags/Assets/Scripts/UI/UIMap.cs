//Unity
using System.Collections; using System.Collections.Generic; using UnityEngine; using System;
using System.Linq;
//HUD
using UnityEngine.UI; using UnityEngine.EventSystems;
//Cities
using GameMechanics.WorldCities;
//Mechanics
using GameMechanics.Data;
using UI.WorldMap;

/// <summary>
/// Main Interface controller in WorldMap view (singleton)
/// Parameters: Panel is On (bool), can Graphic Raycating (bool)
/// Dispatched Events: public static UserClick userclick;
/// Listened Events: MapCamera.ScrollZoom, TimeManager.NewDay
/// </summary>
public class UIMap : MonoBehaviour //, UIDetectable
{
    #region VARIABLES
    //Singleton
    public static UIMap ui;

    //Miscelaneous
    private static Color selectionColor = new Color(1, 0.734f, 0, 1);

    //Graphic Raycast
    private static GraphicRaycaster m_Raycaster;
    private static PointerEventData m_PointerEventData;
    private static EventSystem m_EventSystem;
    public static bool panelON = false;
    public bool canGraphicRaycasting = true;

    //Panels
    [Header("Panels")]

    //----
    // ZOOM
    //----
    //Ventana de Zoom
    [SerializeField] private GameObject _WINDOW_Zoom;
    //Componentes
    private Image _IMG_ZoomFrame, _IMG_ZoomIcon;
    private Text _TEXT_ZoomLevel;

    //----
    // PANEL DE ENCLAVE
    //----
    //Vista de enclave
    [SerializeField] private GameObject _WINDOW_Settlement;
    // Vista de recursos de asentamiento
    [SerializeField] private GameObject _PANEL_ExportsData;
    //Vista de información extendida
    [SerializeField] private GameObject _PANEL_CityDtata;
    //Vista de requistos
    [SerializeField] private GameObject _PANEL_Requirements;
    //Vista de datos de refugio
    [SerializeField] private GameObject _PANEL_ShelterData;

    //----
    // PANEL DE INFORMACIÓN DEL MUNDO
    //----
    [Header("Ventana de Información del Mundo")]
    [SerializeField] private GameObject _WINDOWS_WorldInfoPanel;
    [SerializeField] private Image[] _IMGS_WorldInfoSection;
    [SerializeField] private GameObject[] _PANELS_WorldInfoPanels;
    //Vista de listado de ciudades
    [SerializeField] private GameObject _PREF_CItyRow;
    [SerializeField] private Transform _CONTENT_CityInfoLayout;
    [SerializeField] private Image customFlag;
    // -- Lista de ciudades
    List<MB_City> revealedCities;
        // -- Botón: Ordenar por población
    private bool ascending = false;
        // -- Botón: Ordenar por población
    private bool az = true;
        // -- Dropdown: Ordenar por recursos
    [SerializeField] private Dropdown _DROPDOWN_resourceCityFilter;

    //Vista de Piratas
    [SerializeField] private GameObject _PREF_PirateRow;
    [SerializeField] private Transform _CONTENT_PirateInfoLayout;

    //----
    // VISTA DE ENCLAVES Y CIUDADES
    //----
    [Header("Ventanas de Vista de Ciudades y Enclaves")]
    //Ventana
    [SerializeField] private GameObject _WINDOW_OnCityPort;
    //Elementos de texto en panel
    [SerializeField] private Text _TEXT_CityViewName, _TEXT_CityViewPopulation, _TEXT_CityViewDomain;
    [SerializeField] private GameObject _SCENEOBJ_City;
    //Bandera
    [SerializeField] private MeshRenderer flagRenderer;

    //----
    // MENÚ
    //----
    [SerializeField]
    private GameObject
        _WINDOW_Menu,
        _WINDOW_SETTINGS;

    [Header("Graphic Display")]
    [SerializeField] private Sprite[] _flags, _backgrounds;

    //RESOURCES
    [Header("Resources")]
    [SerializeField] private GameObject _PREF_ResourceView;
    [SerializeField] private Sprite[] _resources;
    private Transform _LAYOUTCONTENT_CityView;

    //Cities Info
    private Text _TEXT_Name, _TEXT_type, _TEXT_SettlementInfo, _TEXT_Population, _TEXT_Requirements, _TEXT_RequirementsLabel, _TEXT_ShelterInfo;
    private Image _IMG_SettlementBackground, _IMG_SettlementSprite, _IMG_SettlementFlag;

    //Calendar
    [Header("Calendar")]
    [SerializeField] private Text _TEXT_Calendar;
    [SerializeField] private Text _TEXT_TimeSpeed;

    //Main Hame Info
    [Header("Main Game's Values")]
    [SerializeField] private Text _TEXT_PlayersGold;
    [SerializeField] private Text _TEXT_PlayersReputation;

    //Miscelaneous
    [SerializeField] private GameObject _deltaMovementIcon;
    [SerializeField] private UIConvoyHUD _UIConvoyHUD;

    //Events
    public delegate void VoidEvent();
    public static VoidEvent userclick, clearPanels;
    public delegate void Transition(bool state);
    public static Transition uiTransition;

#endregion
    private void Awake()
    {
        ui = this;
        //Events
        MapCamera.ScrollZoom += UpdateZoom;
        TimeManager.NewDay += UpdateCalendar;
        PersistentGameData.updateGold += UpdateGold;
    }
    void Start()
    {
        //Graphic Raycast
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();

        //ZOOM PANEL
        _IMG_ZoomFrame = _WINDOW_Zoom.transform.GetChild(0).GetComponent<Image>();
        _IMG_ZoomIcon = _WINDOW_Zoom.transform.GetChild(1).GetComponent<Image>();
        _TEXT_ZoomLevel = _WINDOW_Zoom.transform.GetChild(0).GetChild(0).GetComponent<Text>();

        //CITIES INFO PANEL
        _IMG_SettlementBackground = _WINDOW_Settlement.transform.GetChild(0).GetComponent<Image>();
        _TEXT_Name = _WINDOW_Settlement.transform.GetChild(2).GetComponent<Text>();
        _TEXT_type = _WINDOW_Settlement.transform.GetChild(4).GetComponent<Text>();
        _IMG_SettlementSprite = _WINDOW_Settlement.transform.GetChild(5).GetComponent<Image>();
        _IMG_SettlementFlag = _WINDOW_Settlement.transform.GetChild(6).GetComponent<Image>();
        _TEXT_SettlementInfo = _PANEL_CityDtata.transform.GetChild(4).GetChild(1).GetComponent<Text>();
        _TEXT_Population = _PANEL_CityDtata.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        _TEXT_Requirements = _PANEL_Requirements.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        _TEXT_RequirementsLabel = _PANEL_Requirements.transform.GetChild(1).GetComponent<Text>();
        _TEXT_ShelterInfo = _PANEL_ShelterData.transform.GetChild(0).GetChild(0).GetComponent<Text>();

        //Resources
        _LAYOUTCONTENT_CityView = _PANEL_ExportsData.transform.GetChild(0).GetChild(0);

        //Custom flag
        customFlag.sprite = PersistentGameData._GData_PlayerFlag;
    }
    private void OnDestroy()
    {
        MapCamera.ScrollZoom -= UpdateZoom;
        TimeManager.NewDay -= UpdateCalendar;
        PersistentGameData.updateGold -= UpdateGold;
    }
    //Data Geters
    public Sprite GetFlag(int i) { return _flags[i]; }
    public Sprite GetFlag(EntityType_KINGDOM k) { return _flags[(int)k]; }
    public Sprite GetFlag(string tag)
    {
        switch (tag)
        {
            case "KingdomSpain": return _flags[0];
            case "KingdomPortugal": return _flags[1];
            case "KingdomFrance": return _flags[2];
            case "KingdomDutch": return _flags[3];
            case "KingdomBritain": return _flags[4];
            case "Pirate": return _flags[7];
            default: return null;
        }
    }
    public static Sprite GetResourceSprite(int index)
    {
        return ui._resources[index];
    }

    public void UpdateGold(int gold)
    {
        _TEXT_PlayersGold.text = gold.ToString();
    }

    public void UpdateReputation(int reputation)
    {
        _TEXT_PlayersReputation.text = reputation + " %";
    }
#region INPUTS
    private void Update()
    {
        GetInputs();
    }
    private void GetInputs()
    {
        if (Input.GetMouseButton(0))
        {
            userclick();
        }
        if (Input.GetMouseButtonDown(0))
        {
            userclick();
            //print(userclick.GetInvocationList()[0].Target);
            if (panelON & canGraphicRaycasting)
                if (!GetGraphicRaycastResult()) ClearPanels();
        }
    }
#endregion

#region HUD response
    public static bool GetGraphicRaycastResult()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        //Does the raycast hit a GUI element?
        return results.Count != 0;
    }

    public void DeltaMove(bool val)
    {
        _deltaMovementIcon.SetActive(val);
    }
#endregion

#region ZOOM AND CAMERA
    private void UpdateZoom(float val)
    {
        _TEXT_ZoomLevel.text = "Zoom: " + val + "%";
        _WINDOW_Zoom.SetActive(true);
        StartCoroutine("ZoomFade");
    }
    IEnumerator ZoomFade()
    {
        _IMG_ZoomFrame.color = Color.white;
        _IMG_ZoomIcon.color = Color.white;
        _TEXT_ZoomLevel.color = new Color(_TEXT_ZoomLevel.color.r, _TEXT_ZoomLevel.color.g, _TEXT_ZoomLevel.color.b, 1);
        yield return new WaitForSecondsRealtime(0.3f);
        while (_TEXT_ZoomLevel.color.a > 0)
        {
            yield return new WaitForEndOfFrame();
            _IMG_ZoomFrame.color = new Color(1, 1, 1, _IMG_ZoomFrame.color.a - 0.3f * Time.unscaledDeltaTime);
            _IMG_ZoomIcon.color = new Color(1, 1, 1, _IMG_ZoomIcon.color.a - 0.3f * Time.unscaledDeltaTime);
            _TEXT_ZoomLevel.color = new Color(_TEXT_ZoomLevel.color.r, _TEXT_ZoomLevel.color.g, _TEXT_ZoomLevel.color.b, _TEXT_ZoomLevel.color.a - 0.3f * Time.unscaledDeltaTime);
        }
        _WINDOW_Zoom.SetActive(false);
    }
    public void LockCamera() { MapCamera.isLocked = true; }
    public void UnlockCamera() { MapCamera.isLocked = false; }
#endregion

#region WORLD POINTS INFO
    private void DisplayBasicInfo(KeyPoint target, byte bg, bool isProducing = false)
    {
        _TEXT_Name.text = target.cityName;
        _IMG_SettlementBackground.sprite = _backgrounds[bg];
        _IMG_SettlementSprite.sprite = target.GetSprite();
        panelON = true;
        _WINDOW_Settlement.SetActive(true);
        _PANEL_ExportsData.SetActive(isProducing);
        _PANEL_Requirements.SetActive(!isProducing);
    }
    private string SetRequirements(KeyPoint target)
    {
        if(target is MB_NaturalPort)
        {
            MB_NaturalPort thisPort = (MB_NaturalPort)target;
            _TEXT_RequirementsLabel.text = "Profundidad: <color=blue>Nivel " + (thisPort.calado + 1) + "</color>";
            return thisPort.cityName + "  posee aguas con suficiente profundad para el anclaje de " + thisPort.Calado() + " o barcos de menor calado.";
        }
        else if (target is MB_PirateShelter)
        {
            MB_PirateShelter thisShelter = (MB_PirateShelter)target;
            _TEXT_RequirementsLabel.text = "Derecho de paso";
            return "Para entrar a " + thisShelter.cityName + thisShelter.GetRequirementString();
        }
            return string.Empty;
    }
    public void DisplayInfo(MB_City targetCity)
    {
        if (!_WINDOW_Settlement.activeSelf)
        {
            var targetKingdom = targetCity.transform.parent.parent.GetComponent<Kingdom>();
            DisplayBasicInfo(targetCity, 0, true);
            _TEXT_type.text = "Ciudad " + targetKingdom.GENTILISM_FEMSIN;
            _IMG_SettlementFlag.sprite = _flags[(int)targetKingdom.thisKingdom];

            //City advanced info
            SetResourcesCityDisplay(targetCity);
            _PANEL_CityDtata.SetActive(true);
            _TEXT_SettlementInfo.text = "Tu nivel de Fama en " + targetKingdom.KINGDOMNAME + " es lo bastante bajo para pasar desapercibido en el puerto de " + targetCity.cityName;
            _TEXT_Population.text = targetCity.population.ToString();
            _PANEL_ShelterData.SetActive(false);
        }
    }
    public void DisplayInfo(MB_Town targetTown)
    {
        if (!_WINDOW_Settlement.activeSelf)
        {
            var targetKingdom = targetTown.transform.parent.parent.GetComponent<Kingdom>();
            DisplayBasicInfo(targetTown, 1, true);
            _TEXT_type.text = "Villa " + targetKingdom.GENTILISM_FEMSIN;
            _IMG_SettlementFlag.sprite = _flags[(int)targetKingdom.thisKingdom];

            //City advanced info
            SetResourcesCityDisplay(targetTown);
            _PANEL_CityDtata.SetActive(true);
            _TEXT_SettlementInfo.text = "Las villas y pueblos carecen de patrullas y están despro-\ntegidos. ¡Podemos sembrar el caos y divertirnos, Yahaharl!";
            _TEXT_Population.text = targetTown.population.ToString();
            _PANEL_ShelterData.SetActive(false);
        }
    }
    public void DisplayInfo(MB_NaturalPort targetBay)
    {
        if(!_WINDOW_Settlement.activeSelf)
        {
            var targetKingdom = targetBay.transform.parent.parent.GetComponent<Kingdom>();
            DisplayBasicInfo(targetBay, 2);
            _TEXT_type.text = "Puerto Natural";
            _IMG_SettlementFlag.sprite = _flags[5];
            _PANEL_CityDtata.SetActive(false);
            _TEXT_Requirements.text = SetRequirements(targetBay);
            _PANEL_ShelterData.SetActive(true);
            _TEXT_ShelterInfo.text = "Una entrada de agua que ofrece refugio y sirve como escondite frente a patrullas y corsarios";
        }
    }
    public void DisplayInfo(MB_SmugglersPost targetPost)
    {
        if (!_WINDOW_Settlement.activeSelf)
        {
            var targetKingdom = targetPost.transform.parent.parent.GetComponent<Kingdom>();
            DisplayBasicInfo(targetPost, 3, true);
            _TEXT_type.text = "Escondite de Contrabando";
            _IMG_SettlementFlag.sprite = _flags[6];

            //City advanced info
            SetResourcesCityDisplay(targetPost);
            _PANEL_CityDtata.SetActive(false);
            _PANEL_ShelterData.SetActive(true);
            _TEXT_ShelterInfo.text = "Los escondites de contrabando permiten el comercio seguro fuera de ciudades, pero no hay ninguna taberna cerca";
        }
    }
    public void DisplayInfo(MB_PirateShelter shelter)
    {
        if (!_WINDOW_Settlement.activeSelf)
        {
            var targetKingdom = shelter.transform.parent.parent.GetComponent<Kingdom>();
            DisplayBasicInfo(shelter, 4);
            _TEXT_type.text = "Refugio de Piratas";
            _IMG_SettlementFlag.sprite = _flags[7];
            _PANEL_CityDtata.SetActive(false);
            _TEXT_Requirements.text = SetRequirements(shelter);
            _PANEL_ShelterData.SetActive(true);
            _TEXT_ShelterInfo.text = "Una colonia sin gobierno ni ley que permite descanso y refugio para piratas. ¡Y fulanas, muchas fulanas!";
        }
    }
    private void SetResourcesCityDisplay(Settlement target)
    {
        for (byte i = 0; i < _LAYOUTCONTENT_CityView.childCount; i++)
        {
            Destroy(_LAYOUTCONTENT_CityView.GetChild(i).gameObject);
        }
        for (byte i = 0; i < target.exportsIndex.Length; i++)
        {
            var r = Instantiate(_PREF_ResourceView, _LAYOUTCONTENT_CityView);
            r.GetComponent<Image>().sprite = _resources[target.exportsIndex[i]];
            r.GetComponent<UI.WorldMap.ResourceInfo>().SetResource((byte)target.exportsIndex[i]);
        }
    }
    public void ClearPanels()
    {
        //Event
        clearPanels();

        //Unselect city sprite
        KeyPoint.Unselect();

        //Clear UI elements
        panelON = false;
        _WINDOW_Settlement.SetActive(false);
        _WINDOWS_WorldInfoPanel.SetActive(false);
        _WINDOW_Menu.SetActive(false);
        _WINDOW_Menu.transform.GetChild(2).gameObject.SetActive(false);
        _WINDOW_SETTINGS.SetActive(false);
        if (_WINDOW_OnCityPort.activeSelf) DisplayOffCityViewPanel();

        //Unselect convoy selection
        KeyPoint.Unselect();

        //Clear material Updating for 3D elements
        MatController.StopTiming();
    }
    #endregion

#region WORLD INFO DISPLAY

    public void OrderCitiesListPOP()
    {
        revealedCities = ascending ? revealedCities.OrderBy(c => c.population).ToList() : revealedCities.OrderByDescending(c => c.population).ToList();
        ascending = !ascending;
        CreateNewCityList();
    }
    public void OrderCitiesListAZ()
    {
        revealedCities = az ? revealedCities.OrderBy(c => c.cityName[0]).ToList() : revealedCities.OrderByDescending(c => c.cityName[0]).ToList();
        az = !az;
        CreateNewCityList();
    }
    public void FilterCitiesByResource()
    {
        revealedCities = new List<MB_City>(GameManager.gm.Cities.Where(c => c.revealed).ToList());
        int i = _DROPDOWN_resourceCityFilter.value;
        if(i != 0)
        {
            i = i - 1;
            revealedCities = new List<MB_City>(revealedCities.Where(c => c.Contains(i)).ToList());
        }

        CreateNewCityList();
    }

    public void DisplayWorldInfoPanel()
    {
        panelON = true;
        _WINDOWS_WorldInfoPanel.SetActive(true);
        DisplayWorldViewPanel(0);

        //Reset buttons values
        ascending = false;
        az = true;

        //SetCityRows
        revealedCities = new List<MB_City>(GameManager.gm.Cities.Where(c => c.revealed).ToList());
        //Clear layout and create new rows
        CreateNewCityList();

        //Set Pirates rows
        var pirates = new List<Pirate>(Pirate.GetPiratesList().Where(p => p.seenByPlayer).ToList());
        //Clear layout
        while (_CONTENT_PirateInfoLayout.childCount != 0)
        {
            var dr = _CONTENT_PirateInfoLayout.GetChild(0).gameObject;
            DestroyImmediate(dr);
        }
        foreach(Pirate pirate in pirates)
        {
            var rp = Instantiate(_PREF_PirateRow, _CONTENT_PirateInfoLayout).transform;
            rp.GetChild(1).GetComponent<Text>().text = pirate.CharacterName;
        }
    }
    public void DisplayWorldViewPanel(int index)
    {
        for (int i = 0; i < _PANELS_WorldInfoPanels.Length; i++)
        {
            _PANELS_WorldInfoPanels[i].SetActive(false);
            _IMGS_WorldInfoSection[i].color = Color.white;
        }
        _PANELS_WorldInfoPanels[index].SetActive(true);
        _IMGS_WorldInfoSection[index].color = selectionColor;
        
    }

    private void CreateNewCityList()
    {
        while (_CONTENT_CityInfoLayout.childCount != 0)
        {
            var dr = _CONTENT_CityInfoLayout.GetChild(0).gameObject;
            DestroyImmediate(dr);
        }
        foreach (MB_City city in revealedCities)
        {
            var r = Instantiate(_PREF_CItyRow, _CONTENT_CityInfoLayout).transform;
            r.GetComponent<RowCityList>().thisCity = city;
            r.GetChild(0).GetComponent<Image>().sprite = city.GetSprite();
            r.GetChild(0).localScale = city.transform.localScale.x < 0 ? new Vector3(r.GetChild(0).localScale.x * -1, r.GetChild(0).localScale.y, r.GetChild(0).localScale.z) : r.GetChild(0).localScale;

            string tag = city.transform.parent.tag;
            var i = 0;
            switch (tag)
            {
                case "KingdomSpain": i = 0; break;
                case "KingdomPortugal": i = 1; break;
                case "KingdomFrance": i = 2; break;
                case "KingdomDutch": i = 3; break;
                case "KingdomBritain": i = 4; break;
            }
            r.GetChild(1).GetComponent<Image>().sprite = _flags[i];
            r.GetChild(2).GetComponent<Text>().text = city.cityName;
            r.GetChild(3).GetComponent<Text>().text = city.population.ToString();
        }
    }
    #endregion
#region GAME MENU
    public void OpenGameMenu()
    {
        if (ui.gameObject.activeSelf)
        {
            if (!panelON)
            {
                panelON = true;
                _WINDOW_Menu.SetActive(true);
                if(!_UIConvoyHUD.IsShowingPanel)
                    UIMinimap.SliceDownST();
            }
            else
                ClearPanels();
        }
    }



#endregion
#region WORLD KEYPOINTS VIEW & ACTIONS
    public void DisplayCityViewPanel(MB_City k)
    {
        //Set this as locker panel
        panelON = true;
        //Open window
        _WINDOW_OnCityPort.SetActive(true);
        //Lock World Camera
        LockCamera();
        //Normalize time scale
        TimeManager.NormalizeST();
        //Run 3D materials scale-time updating
        MatController.RunTiming();
        //Set panel values
        _TEXT_CityViewName.text = k.cityName;
        Kingdom thisKingdom = GameManager.gm.GetKingdombyTag(k.transform.parent.tag);
        _TEXT_CityViewDomain.text = $"Dominio {thisKingdom.GENTILISM_MALESIN}";
        _TEXT_CityViewPopulation.text = $"Población - {k.population}";
        //Set Flag
        flagRenderer.material = thisKingdom.nationalFlag;
        //Show port city
        _SCENEOBJ_City.SetActive(true);
        //Hide minimap to avoid on-map clicks detection
        uiTransition(true);
    }
    public void DisplayOffCityViewPanel()
    {
        //Unselect keypoint sprite
        KeyPoint.Unselect();
        //Open window
        _WINDOW_OnCityPort.SetActive(false);
        //Unlock Camera
        UnlockCamera();
        //Restore time scale
        TimeManager.RestartTimeST();
        //Set off port city
        _SCENEOBJ_City.SetActive(false);
        //Stop 3D materials scale-time updating
        MatController.StopTiming();
        //Hide minimap to avoid on-map clicks detection
        uiTransition(false);

        panelON = false;
    }
    #endregion
#region CALENDAR & TIME SPEED

    public void UpdateSpeed(float t)
    {
        _TEXT_TimeSpeed.text = "x " + t.ToString();
    }

    private void UpdateCalendar(DateTime newDate)
    {
        _TEXT_Calendar.text = newDate.ToString("dd MMM yyyy");
    }
#endregion

}

interface UIDetectable
{
    public bool GetGraphicRaycastResult();
}
