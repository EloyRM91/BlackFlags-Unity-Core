//Core
using UnityEngine;
using UnityEngine.EventSystems;
//HUD
using UI.WorldMap;
//Mechanics
using GameMechanics.AI;

/// <summary>
/// An UI eventsystem responsive class, with 3D models view display tasks (Singleton)
/// Parameters: Current Selection (GameObject)
///  ** Constructors: None (Monobehaviour)
///  Listened Events: UIConvoyHUD.playerSelected, UIConvoyHUD.npcSelected
/// </summary>
public class View3D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Selection and cameras
    [SerializeField] private GameObject playerShipCam, playerOnPortCam, npcShipCam, View3D_Port;
    [SerializeField] private Transform camPivot;
    [SerializeField] private Vector3 sunPivot;
    private bool inside;

    //3D View Models
    [Header("Ship Models - CIVIC/MERCHANT")] 
    [SerializeField] private GameObject[] models;
    [SerializeField] private GameObject[] modelsSO;
    [Header("Ship Models - MILITARY/PIRATE")] 
    [SerializeField] private GameObject[] modelsMilitary;
    [SerializeField] private GameObject[] modelsMilitarySO;
    [SerializeField] private GameObject convoy;
    private GameObject currentSelection;

    //singleton
    private static View3D instance;

    //Parameters
    private bool playerSelected, pressing;

    private void Awake()
    {
        //Singleton
        instance = this;
    }

    private void Start()
    {
        currentSelection = models[0];
        //Events
        UIConvoyHUD.playerSelected += PlayerShipCam;
        UIConvoyHUD.npcSelected += NPCShipCam;
    }
    private void OnDestroy()
    {
        //Events
        UIConvoyHUD.playerSelected -= PlayerShipCam;
        UIConvoyHUD.npcSelected -= NPCShipCam;
    }
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            if (inside && playerSelected && !pressing)
            {
                camPivot.RotateAround(sunPivot, Vector3.up, Input.GetAxis("Mouse X") * 700 * Time.fixedDeltaTime);
                MapCamera.isMovingCam = false;
                pressing = true;
            }
            else if (pressing)
            {
                camPivot.RotateAround(sunPivot, Vector3.up, Input.GetAxis("Mouse X") * 700 * Time.fixedDeltaTime);
                MapCamera.isMovingCam = false;
            }
            else
            {
                MapCamera.isMovingCam = true;
                pressing = false;
            }
        }
        else
            pressing = false;
    }
    //Implement interface methods
    public void OnPointerEnter(PointerEventData e)
    {
        inside = true;
        CursorManager.SetCursor(playerSelected ? 4: 5);
    }
    public void OnPointerExit(PointerEventData e)
    {
        inside = false;
        CursorManager.SetCursor(0);
    }
    //------
    //Cameras control
    //------
#region CAMERAS
    /// <summary>
    /// Close player ship view is activated 
    /// </summary>
    public void PlayerShipCam()
    {
        playerSelected = true;

        npcShipCam.SetActive(false);

        if (PlayerMovement.IsInPort())
        {
            playerShipCam.SetActive(false);
            View3D_Port.SetActive(true);
            playerOnPortCam.SetActive(true);
        }
        else
        {
            playerShipCam.SetActive(true);
            View3D_Port.SetActive(false);
            playerOnPortCam.SetActive(false);
        }
    }
    /// <summary>
    /// NPC ship view Camera is activated
    /// </summary>
    public void NPCShipCam()
    {
        playerSelected = false;
        playerShipCam.SetActive(false);
        View3D_Port.SetActive(false);
        playerOnPortCam.SetActive(false);
        npcShipCam.SetActive(true);
    }
#endregion
    //------
    //Models
    //------
#region MODELS
    /// <summary>
    /// Display the correct 3D model, according to a given index
    /// </summary>
    /// <param name="index"></param>
    public static void ShowModel(int index)
    {
        instance.Set(index);
    }
    /// <summary>
    /// Display the correct 3D model, according to a given index
    /// </summary>
    /// <param name="index"></param>
    public static void ShowModel(ClassAI AI, int index, float angle)
    {
        //Set selected model
        instance.Set(index, AI, angle);
    }

    /// <summary>
    /// Display the correct 3D model sailing off, according to a given index
    /// </summary>
    /// <param name="index"></param>
    public static void ShowModelSO(int index)
    {
        instance.SetSO(index);
    }
    private void Set(int index, ClassAI AI = null)
    {
        //Run material Updating for 3D elements
        MatController.RunTiming();

        currentSelection.SetActive(false);
        currentSelection = AI is AI_LocalMerchant || AI is AI_Merchant ? models[index] : modelsMilitary[index];
        currentSelection.SetActive(true);

        //Reset flags
        var flags = currentSelection.transform.GetChild(5);
        for (int i = 0; i < 3; i++)
        {
            flags.GetChild(i).gameObject.SetActive(false);
            
        }
        if(AI != null)
        {
            if (AI is AI_LocalMerchant || AI is AI_Merchant) //----CIVIC FLAGS
            {
                SetKingdomFlags(flags.GetChild(0), AI.transform.tag);
            }
            else if(AI is AI_Pirate) //----------------------------PIRATE FLAG
            {
                flags.GetChild(2).gameObject.SetActive(true);
            }
            else //------------------------------------------------MILITARY FLAGS
            {
                SetKingdomFlags(flags.GetChild(1), AI.transform.tag);
            }
        }
    }
    private void SetSO(int index, ClassAI AI = null)
    {
        //Run material Updating for 3D elements
        MatController.RunTiming();

        currentSelection.SetActive(false);
        currentSelection = AI is AI_LocalMerchant || AI is AI_Merchant ? modelsSO[index] : modelsMilitarySO[index];
        currentSelection.SetActive(true);

        var flags = currentSelection.transform.GetChild(5);
        for (int i = 0; i < 3; i++)
        {
            flags.GetChild(i).gameObject.SetActive(false);

        }
        if (AI != null)
        {
            if (AI is AI_LocalMerchant || AI is AI_Merchant) //----CIVIC FLAGS
            {
                SetKingdomFlags(flags.GetChild(0), AI.transform.tag);
            }
            else if (AI is AI_Pirate) //----------------------------PIRATE FLAG
            {
                flags.GetChild(2).gameObject.SetActive(true);
            }
            else //------------------------------------------------MILITARY FLAGS
            {
                SetKingdomFlags(flags.GetChild(1), AI.transform.tag);
            }
        }
    }
    private void SetKingdomFlags(Transform container, string tag)
    {
        container.gameObject.SetActive(true);
        for (int i = 0; i < container.childCount; i++)
        {
            container.GetChild(i).gameObject.SetActive(false);
        }
        switch (tag)
        {
            case "KingdomSpain": container.GetChild(0).gameObject.SetActive(true); break;
            case "KingdomPortugal": container.GetChild(1).gameObject.SetActive(true); break;
            case "KingdomFrance": container.GetChild(2).gameObject.SetActive(true); break;
            case "KingdomDutch": container.GetChild(3).gameObject.SetActive(true); break;
            case "KingdomBritain": container.GetChild(4).gameObject.SetActive(true); break;
        }
    }
    private void Set(int index, ClassAI AI, float angle)
    {
        Set(index, AI);
        currentSelection.transform.rotation = Quaternion.Euler(0, angle + 180, 0);
    }
    /// <summary>
    /// Display the convoy view 3D models
    /// </summary>
    /// <param name="index"></param>
    public static void ShowConvoy(ClassAI AI, float angle)
    {
        //Set Convoy as selected models group
        instance.SetConvoy(AI, angle);
    }
    private void SetConvoy(ClassAI AI, float angle)
    {
        currentSelection.SetActive(false);
        currentSelection = convoy;
        currentSelection.SetActive(true);

        //Set convoy rotation
        currentSelection.transform.rotation = Quaternion.Euler(0, angle, 0);

        //Reset convoy ships' flags && Assign flags to each ship
        //On every ship...
        for (int i = 0; i < convoy.transform.childCount; i++)
        {
            //this ship's flags group
            var flags = convoy.transform.GetChild(i).GetChild(5);

            //Reset convoy ships' flags
            for (int j = 0; j < 3; j++)
            {
                flags.GetChild(j).gameObject.SetActive(false);
            }

            //Set Flags
            if (AI is AI_Merchant) //European Convoy
            {
                SetKingdomFlags(flags.GetChild(0), AI.transform.tag);
            }
            else if (AI is AI_Pirate) //Military Convoy
            {
                //OWO
            }
        }
    }
#endregion
}
