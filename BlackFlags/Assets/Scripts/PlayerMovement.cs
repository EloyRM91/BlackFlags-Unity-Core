//Unity
using System.Collections; using System.Collections.Generic; using UnityEngine;
//Pathfinding
using UnityEngine.AI; using GameMechanics.AI;
//Cities
using GameMechanics.WorldCities;
//Mechanics
using GameMechanics.Ships;
using GameMechanics.Data;

/// <summary>
/// The player's input and movement controller
/// </summary>
public class PlayerMovement : Convoy
{
    //References
    [Header("REF: World and Minimap")]
    [SerializeField] private Camera _worldCamera;
    [SerializeField] private Camera _minimapCamera;

    //Player Data
    [Header("Player Data")]
    public static Ship playership;
    public static string playerShipName;
    public static string playerName;

    public static bool canMove = true;
    private static bool inPort;
    private ConvoyNPC thisConvoyTarget = null;

    //Route Data
    private static float _remainingTime = 0;
    public static bool onROute = false;

    //Raycast
    private LayerMask _layerMask;
    private NavMeshHit _hit;

    //Events
    // -- destination is set
    public static DestinationEvent playerSetDestination;
    // -- Arrive to destination
    public static VoidEvent ArriveToDestination;
    // -- Arrive to keypoint destination
    public delegate void ArriveToKeypointDestination(KeyPoint port); public static ArriveToKeypointDestination _EVENT_ArriveToPort;
    //Exit from keypoint port
    public static VoidEvent _EVENT_ExitFromPort;
    // -- Drawing path
    public delegate void DestinationPathEvent(Vector3[] path); public static DestinationPathEvent playerSetPath;
    // -- Rotation Event
    public static RotationEvent playerLookRotation;
    // -- Update Route Timer;
    public delegate void SetRouteTime(float routeTime); public static SetRouteTime _EVENT_refreshRouteTime;

    //Path
    [Header("Drawing path in map")]
    [SerializeField] private Transform _drawerPool;
    [SerializeField] private GameObject _ballPrefab;
    private List<GameObject> _activeDots = new List<GameObject>();

    //GetData
    public ConvoyNPC GetTarget() { return thisConvoyTarget; }
    public float GetTime() { return _remainingTime; }
    //public KeyPoint GetCurrentPort() { return currentPort; }

    protected override void Start()
    {
        canMove = true;
        base.Start();
        //Set physics layer mask
        _layerMask = LayerMask.GetMask("KeyPoint", "Default", "Water");
        //Set player ship class
        if (PersistentGameData.currentSceneIsTutorial)
        {
            playership = new ShipSubCategory_MilitaryBalander();
            playerShipName = "Joraique";
            playerName = "Mariel Espinosa";
        }
        else
        {
            playership = PersistentGameData._GData_PlayerShip;
            playerShipName = PersistentGameData._GData_ShipName;
            playerName = PersistentGameData._GData_PlayerName;
        }
        playership.name_Ship = playerShipName;
        playership.name_Captain = playerName;


        //Link the player banner
        _thisConvoySpriteController = UI.WorldMap.PoolingShipSprites.GetPlayerSprite();
        //set the model's sprites
        _thisConvoySpriteController.SetSpritesSet(playership.GetSpriteIndex());
        //Parameters
        SetPlayerParameters();

        //Start on Port
        inPort = true;
        GetComponent<BoxCollider>().enabled = false;
        _clickRay.origin = transform.position + Vector3.up * 3;
        _clickRay.direction = Vector3.up * -1;
        if (Physics.Raycast(_clickRay, out _clickHit, 10, _layerMask))
        {
            if (_clickHit.collider.CompareTag("KeyPoint"))
                currentPort = _clickHit.transform.GetComponent<KeyPoint>();
        }

        //Events 
        MoraleModifier.onSpeedModifier += SetPlayerParameters;
    }
    private void OnDestroy()
    {
        //Events 
        MoraleModifier.onSpeedModifier += SetPlayerParameters;
    }
    //Inputs
    void Update()
    {
        GetInputs();
    }

    private void GetInputs()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (canMove & !UIMap.panelON)
            {
                if (!UIMap.GetGraphicRaycastResult())
                {
                    if (UIMinimap.GetGraphicRaycastResultST()) //hit on minimap
                    {
                        print("le doy al minimapa");
                        TryToGetDestination(_minimapCamera);
                    }
                    else //hit on terrain
                    {
                        TryToGetDestination(_worldCamera);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set player parameters as unit speed.
    /// </summary>
    public void SetPlayerParameters()
    {
        convoySpeed = (1 + MoraleModifier.GetSpeedModifier()) * playership.GetMinSmoothSpeed() / 3;
    }
    private void TryToGetDestination(Camera cam)
    {
        _clickRay.origin = cam.ScreenToWorldPoint(Input.mousePosition);
        _clickRay.direction = Vector3.up * -1;
        CheckRay();
    }
    public override void RecalculateByTargetChanges()
    {
        //perseguidor recibe cambio de rumbo e intenta interceptar de nuevo
        StartCoroutine("RecalculateByChangesSync");
    }
    IEnumerator RecalculateByChangesSync()
    {
        while (thisConvoyTarget == null)
            yield return null;
        CreatePath(Intercept(thisConvoyTarget));
    }
    public override void TargetLost()
    {
        if (thisConvoyTarget != null)
        {
            var chas = thisConvoyTarget.GetComponent<IsTarget>();
            if (chas != null)
            {
                if (chas.chasers.Count == 1)
                {
                    thisConvoyTarget.isOnTarget = false;
                    Destroy(chas);
                }
                else chas.chasers.Remove(this);
            }
            if (thisConvoyTarget.convoyCurrentState == ConvoyState.AtPort) //Target has reached port? Then pursuit is over
            {
                Stop();
            }
            thisConvoyTarget = null;
        }
        ClearDrawing();
        ArriveToDestination();
    }
    private void CheckRay()
    {
        if (Physics.Raycast(_clickRay, out _clickHit, 70, _layerMask))
        {
            if (_clickHit.collider.CompareTag("KeyPoint")) //Hit on Keypoint?
            {
                if(_clickHit.transform.GetComponent<KeyPoint>() != currentPort)
                {
                    if (inPort) ExitCity();
                    currentPort = _clickHit.transform.GetComponent<KeyPoint>();
                    if (thisConvoyTarget != null)
                    {
                        TargetLost();
                        thisConvoyTarget = null;
                    }
                    CreatePath(_clickHit.transform.GetChild(0).position);
                }
            }
            else if (_clickHit.collider.CompareTag("Respawn")) //Hit on Map?
            {
                if(inPort) ExitCity();
                currentPort = null;
                if (thisConvoyTarget != null)
                {
                    var chas = thisConvoyTarget.GetComponent<IsTarget>();
                    if(chas != null)
                    {
                        if (chas.chasers.Count == 1)
                        {
                            thisConvoyTarget.isOnTarget = false;
                            Destroy(chas);
                        }
                        else chas.chasers.Remove(this);
                    }
                    thisConvoyTarget = null;
                }
                CreatePath(_clickHit.point);
                //print(GetNavmeshTerrain(_clickHit.point));
            }
            else //Hit on convoy target
            {
                if (currentPort != null) ExitCity();
                currentPort = null;
                var target = _clickHit.transform.GetComponent<ConvoyNPC>();
                if (PlayerExplorer.IsVisible(target.transform)) //the hit is a visible convoy, and is set as target
                {
                    //thisConvoyTarget = target;
                    target.PointedOut();
                    if (target != null)
                    {
                        CreatePath(Intercept(target));
                    }
                }
                else //the hit is out of vision, go to map point
                {
                    CreatePath(_clickHit.point);
                }
            }
        }
    }
    protected override Vector3 Intercept(Convoy target)
    {
        thisConvoyTarget = (ConvoyNPC)target;
        return base.Intercept(target);
    }
    private int GetNavmeshTerrain(Vector3 position)
    {
        //for (byte i = 0; i < 7; i++)
        //{
        //    if (NavMesh.SamplePosition(position, out hit, 100.0f, i))
        //    {
        //        return hit.mask;
        //    }
        //}
        //return 0;

        var mask = 1 << NavMesh.GetAreaFromName("Coast");
        if (NavMesh.SamplePosition(position, out _hit, 1, mask))
        {
            print("costa");
            return _hit.mask;
        }
        mask = 1 << NavMesh.GetAreaFromName("Sea");
        if (NavMesh.SamplePosition(position, out _hit, 1, mask))
        {
            print("mar");
            return _hit.mask;
        }
        mask = 1 << NavMesh.GetAreaFromName("Ocean");
        if (NavMesh.SamplePosition(position, out _hit, 1, mask))
        {
            print("oc�ano");
            return _hit.mask;
        }
        return 0;
    }
    public void PointAsTarget(Convoy target)
    {
        CreatePath(Intercept(target));
    }
    private void CreatePath(Vector3 d)
    {
        if ((d - transform.position).sqrMagnitude < 0.25f) //too close to tracing a new path
        {
            ClearDrawing();
            ArriveToDestination();
        }
        else
        {
            if (CanGoTo(d))
            {
                Stop();
                //Set Destination
                _destination = d;
                //Call delegate
                playerSetDestination(d);
                //SetPath
                DrawPath(path.corners);
                //Move
                Move((path.corners));

                if (thisConvoyTarget != null)
                {
                    var chas = thisConvoyTarget.GetComponent<IsTarget>();
                    if (chas == null)
                    {
                        chas = thisConvoyTarget.gameObject.AddComponent<IsTarget>();
                        chas.chasers.Add(this);
                        thisConvoyTarget.isOnTarget = true;
                    }
                    else
                    {
                        if (!chas.chasers.Contains(this))
                            chas.chasers.Add(this);
                    }
                }
            }
            else if (thisConvoyTarget != null)
            {
                print("no puede interceptar en ruta actual con una intercepci�n perfecta as� que busca la mejor ruta optativa");
                Stop();
                byte i = 5;
                var targetPos = thisConvoyTarget.transform.position;
                var dir = thisConvoyTarget.transform.forward;
                while (i > 0)
                {

                    if (CanGoTo(targetPos + dir * i))
                    {
                        CreatePath(targetPos + dir * i);
                        return;
                    }
                    i--;   
                } 
                playerSetDestination(targetPos);
                CreatePath(targetPos);
            }
            else //BUG PATCHER
            {
                print("burgir");
                //Stop();
                //currentPort = null;
            }
        } 
    }
    protected override void Move(Vector3[] corners)
    {
        //Player starts route
        onROute = true;
        //Refesh route state
        StartCoroutine("RefreshRouteTime");
        //Set Route and movement
        StartCoroutine(CheckDestination());
        StartCoroutine("Movement");
        //Calculate Time now
        _remainingTime = CalculateRouteTime();
        _EVENT_refreshRouteTime(_remainingTime);
        //print($"Tiempo: {_remainingTime} d�as");

        GameManager.gm.getCurrentEffect(transform);
    }
    public override Vector3 GetDestination() { return _destination; }
    IEnumerator CheckDestination()
    {
        Vector3[] waypoints = path.corners;
        transform.rotation = Quaternion.LookRotation(waypoints[1] - transform.position);
        _thisConvoySpriteController.SetSprite();
        playerLookRotation(transform.rotation);

        pathProgressIndex = 1;
        while(pathProgressIndex < waypoints.Length)
        {
            _destination = waypoints[pathProgressIndex];
            transform.rotation = Quaternion.LookRotation(waypoints[pathProgressIndex] - transform.position);
            if (Vector3.Distance(transform.position, _destination) < 0.215f)
            {
                if (thisConvoyTarget != null)
                {
                    if (Vector3.Distance(transform.position, thisConvoyTarget.transform.position) < 0.215f)
                    {
                        //alcanza el objetivo
                        ClearDrawing();
                        ArriveToDestination();
                        Stop();
                        //print("chased");
                    }
                    else if(pathProgressIndex < waypoints.Length - 1 && pathProgressIndex != 1)
                    {
                        if(Vector3.Distance(_destination, waypoints[pathProgressIndex + 1]) > 0.5f)
                        {
                            var tempDest = Intercept(thisConvoyTarget);
                            transform.rotation = Quaternion.LookRotation(waypoints[pathProgressIndex + 1] - transform.position);
                            _thisConvoySpriteController.SetSprite();
                            CreatePath(tempDest);
                        }
                    }
                }
                pathProgressIndex++;
                if (pathProgressIndex < waypoints.Length)
                {
                    transform.rotation = Quaternion.LookRotation(waypoints[pathProgressIndex] - transform.position);
                    _thisConvoySpriteController.SetSprite();
                    playerLookRotation(transform.rotation);
                }   
                else //llega a destino
                {
                    //Borra la l�nea de rumbo trazado
                    ClearDrawing();
                    //Avisa a los crossmap que ya no hay un destino
                    ArriveToDestination();
                    //Detiene el movimiento
                    Stop();
                    //Si el destino es puerto, entra a puerto
                    if (currentPort != null) EnterCity();
                }
            }
            yield return new WaitForSeconds(0.04f);
        }
        Stop();
    }
#region PATH LINE

    /// <summary>
    /// Set a sprites trail according to the current path
    /// </summary>
    /// <param name="positions"></param>
    private void DrawPath(Vector3[] positions)
    {
        ClearDrawing();
        var density = 1.2f;
        if(positions.Length > 1)
        {
            for (int i = 1; i < positions.Length; i++)
            {
                var dis = Vector3.Distance(positions[i - 1], positions[i]);
                var balls = (int)dis / density;
                var l = dis / balls;
                var initPos = positions[i - 1];
                var dir = Vector3.Normalize(positions[i] - initPos);
                for (int j = 0; j < balls; j++)
                {
                    var newBallPosition = initPos + density * j * dir;
                    var _ball = GetBall();
                    _ball.transform.position = newBallPosition;
                }
            }
        }
    }

    /// <summary>
    /// Get a blue-ball sprite from the pool
    /// </summary>
    /// <returns></returns>
    private GameObject GetBall()
    {
        GameObject ball;
        for (int i = 0; i < _drawerPool.childCount; i++)
        {
            ball = _drawerPool.GetChild(i).gameObject;
            if (!ball.gameObject.activeSelf)
            {
                ball.gameObject.SetActive(true);
                _activeDots.Add(ball);
                return ball;
            }
        }
        ball = Instantiate(_ballPrefab, Vector3.zero, Quaternion.Euler(90, 0, 0), _drawerPool);
        _activeDots.Add(ball);
        return ball;
    }

    /// <summary>
    /// ClearDrawing: Clear the existing path on the map
    /// </summary>
    private void ClearDrawing()
    {
        if (_activeDots.Count != 0)
        {
            foreach (GameObject dot in _activeDots)
            {
                dot.SetActive(false);
            }
            _activeDots = new List<GameObject>();
        }

    }
#endregion

#region ROUTE
    IEnumerator RefreshRouteTime()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            if (onROute)
            {
                //Refresh time to destination
                _remainingTime = CalculateRouteTime();
                //Call delegate
                _EVENT_refreshRouteTime(_remainingTime);
            }
        }
    }

    private void Stop()
    {
        onROute = false;
        StopAllCoroutines();
    }
#endregion

#region PORT AND CITIES
    public MB_City GetPort()
    {
        if(currentPort is MB_City) return (MB_City)currentPort;
        return null;
    }

    public static bool IsInPort()
    {
        return inPort;
    }

    private void EnterCity()
    {
        inPort = true;
        //Add player to settlement's convoys list
        if (currentPort is MB_City)
        {
            var port = GetPort();
            port.convoysInThisPort.Add(this);
        }
        else if(currentPort is MB_PirateShelter)
        {
            var shelter = (MB_PirateShelter)currentPort;
            shelter.convoysInThisPort.Add(this);
        }
        _EVENT_ArriveToPort(currentPort);
        Dissappear(); //Parche para que el barco desaparezca, ya que llamar a este m�todo desde el explorer no hace nada
        GetComponent<BoxCollider>().enabled = false;
    }
    private void ExitCity()
    {
        inPort = false;
        //Add player to settlement's convoys list
        if (currentPort is MB_City)
        {
            var port = (MB_City)currentPort;
            port.convoysInThisPort.Remove(this);
        }
        else if (currentPort is MB_PirateShelter)
        {
            var shelter = (MB_PirateShelter)currentPort;
            shelter.convoysInThisPort.Remove(this);
        }
        GetComponent<BoxCollider>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        _EVENT_ExitFromPort();
    }
#endregion
}

public enum TerrainType { Coast, Sea, Ocean, Reef, None}
