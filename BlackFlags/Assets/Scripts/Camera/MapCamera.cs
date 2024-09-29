//Core
using UnityEngine;
//UI
using UI.WorldMap;
//Native plugins
using System.Runtime.InteropServices;
//Mechanics
using GameMechanics.Data;
//<>
/// <summary>
/// The zoom and camera movement controller
/// Parameters: speed(float), scrollSensivity(50)
/// Inputs: Mouse X, Mouse Y, Scroll
///  ** Constructors: None (Monobehaviour)
/// </summary>
public class MapCamera : MonoBehaviour
{
    #region VARIABLES
    //Borders
    [SerializeField] private static readonly float _XMin = -129, _XMax = 127, _YMin = -56.5f, _YMax = 135;
    //Inputs
    private float _inputX, _inputY, _deltaScroll, _MouseX, _MouseY;
    private bool _deltaPress;
    private bool _startedMoving;
    public static bool isMovingCam;
    //References
    [SerializeField] private Transform _playerRef;

    //Settings
    private float _speed = 20;
    private float _scrollSensivity = 250;
    //Events
    public delegate void Scroll(float value);
    /// <summary>
    /// Event launched when player makes zoom
    /// </summary>
    public static event Scroll ScrollZoom;

    //Camera Data
    private Camera _camera;
    private float _ortographicInitialSize;
    public static bool isLocked;

    //Minimap frame
    private Transform _minimapFrame;
    #endregion

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);

    void Start()
    {
        //Camera
        _camera = transform.GetChild(0).GetComponent<Camera>();
        _ortographicInitialSize = _camera.orthographicSize;

        //Minimap:
        _minimapFrame = transform.GetChild(0).GetChild(0);

        //Events:
        View3D.Focus += FocusOnTarget;
    }
    void Update()
    {
        GetInputs();
        if (!isLocked) Zoom();
    }
    private void FixedUpdate()
    {
        if (!isLocked) Move();
    }
    private void GetInputs()
    {
        _inputX = Input.GetAxisRaw("Horizontal");
        _inputY = Input.GetAxisRaw("Vertical");
        _MouseX = Input.GetAxisRaw("Mouse X");
        _MouseY = Input.GetAxisRaw("Mouse Y");
        _deltaScroll = Input.mouseScrollDelta.y;
        _deltaPress = Input.GetMouseButton(2);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Close UI elements
           
            if (UIMap.ui.gameObject.activeSelf)
            {
                UIMap.ui.ClearPanels();
                //Adjust Camera
                FocusOnTarget(_playerRef);
                //Select player
                UISelector.instance.SetTarget(_playerRef);
                //Sound
                GameMechanics.Sound.Ambience.SelectionSails_ST();
            }

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIMap.ui.OpenGameMenu();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            TimeManager.PauseST();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIMap.ui.DisplayWorldInfoPanel();
        }
    }

    public static void FocusOnTarget(Transform target)
    {
        var cam = Cameras.MainCamera.transform.parent;
        cam.position = new Vector3(target.position.x, cam.position.y, target.position.z);

        //Borders
        if (cam.position.x < _XMin) cam.position = cam.position - cam.right * (_XMin - cam.position.x);
        if (cam.position.x > _XMax) cam.position = cam.position - cam.right * (_XMax - cam.position.x);

        if (cam.position.z < _YMin) cam.position = cam.position - cam.up * (_YMin - cam.position.z);
        if (cam.position.z > _YMax) cam.position = cam.position - cam.up * (_YMax - cam.position.z);
    }

    private void Move()
    {
        //Keyword Input
        Vector3 move = (Vector3.right * _inputX + Vector3.up * _inputY) * _speed * Time.unscaledDeltaTime;
        transform.Translate(move);

        //Mouse input
        if (Input.mousePosition.x < 8) transform.Translate(Vector3.right * -_speed * Time.unscaledDeltaTime);
        if (Input.mousePosition.x > 0.99f * Screen.width) transform.Translate(Vector3.right * _speed * Time.unscaledDeltaTime);
        if (Input.mousePosition.y < 8) transform.Translate(Vector3.up * -_speed * Time.unscaledDeltaTime);
        if (Input.mousePosition.y > 0.99f * Screen.height) transform.Translate(Vector3.up * _speed * Time.unscaledDeltaTime);

        if (_deltaPress && isMovingCam)
        {
            move = (Vector3.right * _MouseX * -1 + Vector3.up * _MouseY * -1) * 8 * Time.unscaledDeltaTime;
            transform.Translate(move);

            var mousePos = Input.mousePosition;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            //UIMap.ui.DeltaMove(true);
            CursorManager.SetCursor(4);
            _startedMoving = true;
            //SetCursorPos((int)mousePos.x, (int)mousePos.y);
        }
        else if (_startedMoving)
        {
            _startedMoving = false;
            CursorManager.SetCursor(0);
            //UIMap.ui.DeltaMove(false);
            Cursor.lockState = CursorLockMode.None;
        }


        //Borders
        var offsetCam = _ortographicInitialSize - _camera.orthographicSize;
        var xmin = _XMin - 1.25f * (offsetCam);
        var xmax = _XMax + 1.25f * (offsetCam);
        var ymin = _YMin - offsetCam;
        var ymax = _YMax + offsetCam;
        if (transform.position.x < xmin) transform.position = transform.position - transform.right * (xmin - transform.position.x);
        if (transform.position.x > xmax) transform.position = transform.position - transform.right * (xmax - transform.position.x);

        if (transform.position.z < ymin) transform.position = transform.position - transform.up * (ymin - transform.position.z);
        if (transform.position.z > ymax) transform.position = transform.position - transform.up * (ymax - transform.position.z);
    }

    private void Zoom()
    {
        if (!GameManager.forcedPause)
        {
            if (_deltaScroll != 0)
            {
                if (!UIMap.GetGraphicRaycastResult())
                {
                    _camera.orthographicSize -= Mathf.Round(_deltaScroll * _scrollSensivity * Time.unscaledDeltaTime);
                    _minimapFrame.localScale *= 1 + -1 * Mathf.Round(_deltaScroll * _scrollSensivity / 10 * Time.unscaledDeltaTime);

                    if (_camera.orthographicSize < 7.5f)
                    {
                        _camera.orthographicSize = 7.5f;
                        _minimapFrame.localScale = Vector3.one * 5.76f;
                    }
                    else if (_camera.orthographicSize > 16f)
                    {
                        _camera.orthographicSize = 16f;
                        _minimapFrame.localScale = Vector3.one * 10.8f;
                    }
                    float p = _camera.orthographicSize > _ortographicInitialSize ? 100 - ((_camera.orthographicSize - _ortographicInitialSize) / _ortographicInitialSize) * 100 : 100 + ((_ortographicInitialSize - _camera.orthographicSize) / _ortographicInitialSize) * 100;
                    ScrollZoom(p);
                }
            }
        }
    }
}
