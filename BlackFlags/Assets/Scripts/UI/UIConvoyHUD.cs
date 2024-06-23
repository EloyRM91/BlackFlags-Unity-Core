//Core
using UnityEngine;
using UnityEngine.UI;
//Mechanics
using GameMechanics.Ships;
using GameMechanics.Data;
using GameMechanics.AI;
//Tweening
using DG.Tweening;

namespace UI.WorldMap
{
    /// <summary>
    /// A HUD manager with responsive ship selection tools for the bottom world map panel
    /// Parameters: Convoy Target (Convoy)
    ///  ** Constructors: None (Monobehaviour)
    ///  Dispatched Events: 
    ///  public static EntitySelected playerSelected, npcSelected
    ///  Listened Events:
    ///  UISelector._EVENT_SelectConvoy, UISelector._EVENT_UnselectConvoy, PlayerMovement._EVENT_refreshRouteTime
    /// </summary>
    public class UIConvoyHUD : MonoBehaviour
    {
#region VARIABLES
        //References
        private static PlayerMovement playerRef;

        //------
        //Display
        //------
        [Header("Sprites del display izquierdo")]
        [SerializeField] private Sprite[] ShipsSprites;
        [SerializeField] private Sprite SPRITE_Convoy;
        [Header("Componentes del layout")]
        [SerializeField] private Image _IMG_LabelImage;
        [SerializeField] private Image _IMG_ShipImage;
        [SerializeField] private Image _IMG_Flag;
        [SerializeField] private Text _TEXT_ShipName, _TEXT_ShipModel, _Text_Role, _TEXT_CaptainName, _Text_Destination, _Text_Speed;
        private Vector3 _offset;

        //Convoy layout
        [Header("Scroll vertical de convoy")]
        [SerializeField] private Transform layoutContainer;
        [SerializeField] private GameObject PREFAB_ShipRow;

        //Buttons
        [SerializeField] private Button _Button_FocusCam, _ButtonSetAsTarget;
        [Header("Botones del display horizontal")]
        [SerializeField] private Button[] displayButtons;
        [Header("Paneles de display de información")]
        [SerializeField] private GameObject[] displayViews;
        
        //Inventory
        [Header("Recursos y Vista de Mercancía")]
        [SerializeField] private Text _TEXT_CurrentPlayerLoad;
        [SerializeField] private Text _TEXT_ShipCapacity;
        [SerializeField] private Transform _resourcesContainer;
        [SerializeField] private Sprite[] _resources, _resourcesEmpty;

        //Weapons
        [Header("Armamento")]
        [SerializeField] private Text _TEXT_Armory;
        [SerializeField] private Text
            //Armas de mano
            _TEXT_Guns_ONLOAD,
            //Falconetes -- Carga / Equipados
            _TEXT_Falconets_ONLOAD, _TEXT_Falconets_EQ,
            //Cañones de ocho libras -- Carga / Equipados
            _TEXT_8lbCannons_ONLOAD, _TEXT_8lbCannons_EQ,
            //Cañones de doce libras -- Carga / Equipados
            _TEXT_12lbCannons_ONLOAD, _TEXT_12lbCannons_EQ,
            //Cañones de 24 libras -- Carga / Equipados
            _TEXT_24lbCannons_ONLOAD, _TEXT_24lbCannons_EQ; 
        [SerializeField] private Transform _weaponsContainer;

        //Crew and morale:
        [SerializeField] private Image
            //Barra de moral por alimentación y bebida
            _FILLER_MoraleSupplies,
            //Barra de moral por saqueo y pillaje
            _FILLER_MoralePillage,
            //Barra de moral por descanso en tierra
            _FILLER_Rest,
            //Icono de nivel de moral
            _IMG_Morale;
        [SerializeField]
        private Text
            _TEXT_MoralBySupplies,
            _TEXT_MoralByPillage,
            _TEXT_MoraleByRest,
            _TEXT_Morale_GLOBAL,
            _TEXT_CrewAmount,
            _TEXT_Onboard,
            _TEXT_OnPanel_MoraleByModifiers,
            _TEXT_OnPanel_MoraleByEvents,
            _TEXT_OnPanel_MoraleGLOBAL;
        [SerializeField] private Sprite[] _moods;

        //Data
        private Convoy _convoyTarget;
        private bool isShowingPanel;

        public bool IsShowingPanel
        {
            get { return isShowingPanel; }
        }

        //Tweening
        private Tween _tween;

        //Events
        public delegate void EntitySelected();
        /// <summary>
        /// Event launched when player has been selected
        /// </summary>
        public static EntitySelected playerSelected;
        /// <summary>
        /// Event launched when a NPC convoy has been selected
        /// </summary>
        public static EntitySelected npcSelected;

#endregion
        private void Awake()
        {
            for (byte i = 0; i < _resourcesContainer.childCount; i++)
            {
                _resourcesContainer.GetChild(i).GetComponent<ResourceInfo>().SetResource(i);
            }
            for (byte i = 0; i < _weaponsContainer.childCount; i++)
            {
                _weaponsContainer.GetChild(i).GetComponent<ResourceInfo>().SetResource((byte)(i + 16));
            }
        }
        private void OnEnable()
        {
            RefreshArmoryInventory();
        }
        void Start()
        {
            //Ref
            playerRef = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
            Row_Ship.display = this;
            //Data
            _offset = transform.position;
            transform.position -= Vector3.up * 600;
            RefreshArmoryInventory();

            //------------
            // EVENTS
            //------------
            //Convoy is selected
            UISelector._EVENT_SelectConvoy += DisplaySelectionInfo;
            //Convoy is unselected
            UISelector._EVENT_UnselectConvoy += ClearDisplayPanel;
            //Route time has changed
            PlayerMovement._EVENT_refreshRouteTime += SetPlayerRouteText;
            //Changes on armory
            ShipInventory.OnEquipmentEditRefresh += RefreshEquipmentRow;
            //Refresh inventory
            ShipInventory.updateLoad += CheckInventory;
            //Refresh morale levels
            ShipInventory.updateMorale += CheckMorale;
        }
        private void OnDestroy()
        {
            //Events
            UISelector._EVENT_SelectConvoy -= DisplaySelectionInfo;
            UISelector._EVENT_UnselectConvoy -= ClearDisplayPanel;
            PlayerMovement._EVENT_refreshRouteTime -= SetPlayerRouteText;
            ShipInventory.OnEquipmentEditRefresh -= RefreshEquipmentRow;
            ShipInventory.updateLoad -= CheckInventory;
            ShipInventory.updateMorale -= CheckMorale;
        }

#region BUTTONS
        private void LockButtons()
        {
            for (int i = 0; i < 5; i++)
			{
                displayButtons[i].interactable = false;
                displayViews[i].SetActive(false);
            }
        }
        private void UnlockButtons()
        {
            for (int i = 0; i < 5; i++)
            {
                displayButtons[i].interactable = true;
            }
        }
        private void ActivateDisplayButton(byte index)
        {
            displayButtons[index].interactable = true;
        }

        public void OpenViewDisplay(int index)
        {
            ClearDisplay();
            displayViews[index].SetActive(true);
        }

        public void ClearDisplay()
        {
            for (int i = 0; i < 5; i++)
            {
                displayViews[i].SetActive(false);
            }
        }
        private void Intercept(Convoy convoy)
        {
            playerRef.PointAsTarget(convoy);
            convoy.PointedOut();
        }

#endregion
        private void DisplaySelectionInfo(Convoy convoy)
        {
            _convoyTarget = convoy;
            //Button Callbacks
            _Button_FocusCam.onClick.RemoveAllListeners();
            _Button_FocusCam.onClick.AddListener(() => MapCamera.FocusOnTarget(convoy.transform));
            _ButtonSetAsTarget.onClick.RemoveAllListeners();
            _ButtonSetAsTarget.onClick.AddListener(() => Intercept(convoy)) ;
            //Move up
            ActivateDisplayPanel();
            //Lock display buttons
            LockButtons();

            if (convoy is ConvoyNPC) //NPC
            {
                //Call Event:
                npcSelected();
                //This convoy's data
                var kingdom = GameManager.gm.GetKingdombyTag(convoy.transform.tag);
                var ai = convoy.GetComponent<ClassAI>();
                //Convoy Role
                _Text_Role.text = $"{ai.GetAIRol()} {ai.GetGentilism(kingdom)}";
                //This convoy can be set as target
                _ButtonSetAsTarget.interactable = true;
                //Explicit conversion
                var c = (ConvoyNPC)convoy;

                if (c.thisConvoyShips.Length == 1) //Single ship
                {
                    ActivateDisplayButton(0);
                    //Show Ship View display
                    OpenViewDisplay(0);
                    //Header color
                    _IMG_LabelImage.color = Color.black;
                    var ship = c.thisConvoyShips[0];
                    SetShipData(c, ship);

                    //SHOW SHIP MODEL
                    View3D.ShowModel(ai, ship.GetSpriteIndex() - 1, GetRelativeAngle(c.transform));
                }
                else //An entire convoy
                {
                    //Display this convoy data
                    SetConvoyData(c);
                    //SHOW SHIP MODEL
                    View3D.ShowConvoy(ai,GetRelativeAngle(c.transform));
                }
            }
            else //Player
            {
                //Call Event
                playerSelected();  //Player is selected
                //Unlock all the buttons! 
                UnlockButtons();
                //Show
                OpenViewDisplay(0);
                //Player can't intercept himself!
                _ButtonSetAsTarget.interactable = false;
                //Set Header Color
                _IMG_LabelImage.color = Color.blue;
                //Set Display Data;
                PlayerData();
                //Convoy name
                _Text_Role.text = "Respetable pirata";
                //Vista de embarcación
                var t = playerRef.GetTime();
                SetPlayerRouteText(t);

                //Showing ship model
                if (PlayerMovement.IsInPort())
                {
                    View3D.ShowModelSO(PlayerMovement.playership.GetSpriteIndex() - 1);
                }
                else
                {
                    View3D.ShowModel(PlayerMovement.playership.GetSpriteIndex() - 1);
                }
                
                //Convoy view
                CreateShipsLayout();

                //Resources
                CheckInventory();

                //Crew Morale & Supplies
                _TEXT_CrewAmount.text = $"Tripulates del {PersistentGameData._GData_ShipName}: {ShipInventory.Crew} miembros";
                _TEXT_Onboard.text = $"Vida a bordo del {PersistentGameData._GData_ShipName}";
                CheckMorale();

                //Weapons
                _TEXT_Armory.text = CheckArmory();
            }
        }

        private void SetConvoyCaptainInDisplay(ConvoyNPC convoy)
        {
            _TEXT_CaptainName.text = "Capitán " + convoy.thisConvoyShips[0].name_Captain;
        }
        public void ActivateDisplayPanel()
        {
            isShowingPanel = true;
            _tween.Kill();
            transform.position = _offset - Vector3.up * 600;
            _tween = transform.DOMove(_offset, 0.3f).SetUpdate(true); 
        }
        private void ClearDisplayPanel()
        {
            isShowingPanel = false;
            _tween.Kill();
            _tween = transform.DOMove(_offset - Vector3.up * 600, 0.3f).SetUpdate(true); 
        }
        private void SetShipData(Convoy c, Ship ship)
        {
            _IMG_Flag.sprite = UIMap.ui.GetFlag(c.transform.tag);
            SetShipViewData(ship);

            GetRelativeDisplacement(c);
            _Text_Speed.text = $"{ship.GetMinSmoothSpeed()} nudos.";
        }

        private void SetConvoyData(ConvoyNPC convoy)
        {
            //Show SConvoy View display
            OpenViewDisplay(1);
            //Convoy button is ow interactable
            ActivateDisplayButton(1);
            //Create a shiplist layout
            CreateShipsLayout(convoy);
            //Header color
            _IMG_LabelImage.color = Color.black;
            //Sprite
            _IMG_ShipImage.sprite = SPRITE_Convoy;
            //Convoy name
            _TEXT_ShipName.text = "Convoy " + convoy.thisConvoyShips[0].name_Ship;
            //Flag color
            _IMG_Flag.sprite = UIMap.ui.GetFlag(convoy.transform.tag);
            //Convoy's captain name
            SetConvoyCaptainInDisplay(convoy);
            //Convoy speed
            _Text_Speed.text = $"{Mathf.Clamp(convoy.convoySpeed, 2, 20)} nudos.";

            GetRelativeDisplacement(convoy);
        }
        public void SetShipViewData(Ship ship)
        {
            _IMG_ShipImage.sprite = ShipsSprites[ship.GetSpriteIndex() - 1];
            _TEXT_ShipName.text = ship.name_Ship;
            _TEXT_ShipModel.text = Ship.GetCompleteName(ship);
            _TEXT_CaptainName.text = "Capitán " + ship.name_Captain;
            _TEXT_ShipCapacity.text = $"Carga: ? -- {ship.GetCapacity()} toneladas";
            _Text_Speed.text = $"{ship.GetMinSmoothSpeed()} nudos.";
        }
        private void GetRelativeDisplacement(Convoy convoy)
        {
            var playerRot = playerRef.transform.rotation;
            var targetRot = convoy.transform.rotation;
            var dirRot = Quaternion.LookRotation(convoy.transform.position - playerRef.transform.position);
            var ang1 = Quaternion.Angle(targetRot, dirRot);
            var ang2 = Quaternion.Angle(playerRot, dirRot);

            //Are both ships geting closer?
            _Text_Destination.text = (ang1 > 90 && ang2 < 90) || (ang1 > 60 && ang2 < 40) ? "Acercándose" : "Alejándose";

            //Is Player chasing?
            if (ang1 < 45 && ang2 < 45) 
                _Text_Destination.text = playerRef.convoySpeed > convoy.convoySpeed ? "Acercándose (por velocidad)" : "Alejándose (por velocidad)";
            //Is Player being chase?
            else if (ang1 > 145 && ang2 > 145)
                _Text_Destination.text = convoy.convoySpeed > playerRef.convoySpeed ? "Acercándose (por velocidad)" : "Alejándose (por velocidad)";
        }
#region SHOW PLAYER DATA
        private void PlayerData()
        {
            var ship = PlayerMovement.playership;
            _IMG_ShipImage.sprite = ShipsSprites[ship.GetSpriteIndex() - 1];
            _IMG_Flag.sprite = PersistentGameData._GData_PlayerAvatar;
            _TEXT_ShipName.text = PlayerMovement.playerShipName;
            _TEXT_ShipModel.text = Ship.GetCompleteName(ship);
            _TEXT_CaptainName.text = "Capitán " + PlayerMovement.playerName;
            _Text_Speed.text = $"{ship.GetMinSmoothSpeed()} nudos.";
            _TEXT_CurrentPlayerLoad.text = $"{ShipInventory.shipLoad} / {ship.GetCapacity()} ton";
            _TEXT_ShipCapacity.text = $"{ShipInventory.shipLoad} / {ship.GetCapacity()} ton";
        }

        private void SetPlayerRouteText(float routeTime)
        {
            if (_convoyTarget == playerRef)
            {
                var txt = playerRef.currentPort != null ? playerRef.currentPort.cityName : "Mar Caribe";
                txt += routeTime == 1 ? $" - {routeTime} día." : $" - {routeTime} días.";
                _Text_Destination.text = txt;
            }
        }
#endregion

#region SHIPS LAYOUT
        private void CreateShipsLayout(ConvoyNPC convoy)
        {
            ClearConvoyLayout();
            var ships = convoy.thisConvoyShips;
            for (int i = 0; i < ships.Length; i++)
            {
                GameObject row = Instantiate(PREFAB_ShipRow, layoutContainer);
                row.GetComponent<Row_Ship>().SetData(ships[i]);
            }
        }

        private void CreateShipsLayout()
        {
            ClearConvoyLayout();
            var ship = PlayerMovement.playership;
            GameObject row = Instantiate(PREFAB_ShipRow, layoutContainer);
            row.GetComponent<Row_Ship>().SetData(ship);
        }

        private void ClearConvoyLayout()
        {
            while(layoutContainer.childCount != 0)
            {
                DestroyImmediate(layoutContainer.GetChild(0).gameObject);
            }
        }

        public void UnselectAllRows()
        {
            ActivateDisplayButton(0);
            for (int i = 0; i < layoutContainer.childCount; i++)
            {
                layoutContainer.GetChild(i).GetComponent<Row_Ship>().Unselect();
            }
            
        }
#endregion

#region INVENTORY
        private void CheckInventory()
        {
            var items = ShipInventory.Items;
            for (int i = 0; i < 16; i++)
            {
                var r = _resourcesContainer.GetChild(i);
                if (items[i] != 0)
                {
                    r.GetComponent<Image>().sprite = _resources[i];
                    r.GetChild(0).GetComponent<Text>().text = items[i].ToString();
                }
                else
                {
                    r.GetComponent<Image>().sprite = _resourcesEmpty[i];
                    r.GetChild(0).GetComponent<Text>().text = string.Empty;
                }
            }

            for (int i = 16; i < 21; i++)
            {
                var r = _weaponsContainer.GetChild(i - 16);
                if (items[i] != 0)
                {
                    r.GetComponent<Image>().sprite = _resources[i];
                    r.GetChild(0).GetComponent<Text>().text = items[i].ToString();
                }
                else
                {
                    r.GetComponent<Image>().sprite = _resourcesEmpty[i];
                    r.GetChild(0).GetComponent<Text>().text = string.Empty;
                }
            }
        }
        public static string CheckArmory()
        {
            var ship = PlayerMovement.playership;
            var armoryCapacity = ship.GetPowerCapacity();
            string targetText = $"{ship.name_Ship} tiene porte para {armoryCapacity[0]} falconetes en cubierta";

            byte numCosas = 0;
            for (byte i = 1; i < 4; i++)
            {
                if (armoryCapacity[i] > 0) numCosas++;
            }
            byte j = 1;
            while (numCosas != 0)
            {
                targetText += numCosas == 1 ? " y " : ", ";
                var n = armoryCapacity[j];
                if (n != 0)
                {
                    targetText += $"{n} {EconomyBehaviour.GetWeaponName(j)}";
                    j++;
                    numCosas--;
                }
            }

            return targetText;


        }

        private void RefreshArmoryInventory()
        {
            var onLoadArmory = ShipInventory.Items;
            var equipment = ShipInventory.equipment;
            //row 1
            _TEXT_Guns_ONLOAD.text = onLoadArmory[16].ToString();
            //row 2
            _TEXT_Falconets_ONLOAD.text = (onLoadArmory[17] - equipment[0]).ToString();
            _TEXT_Falconets_EQ.text = equipment[0].ToString();
            //row 3
            _TEXT_8lbCannons_ONLOAD.text = (onLoadArmory[18] - equipment[1]).ToString();
            _TEXT_8lbCannons_EQ.text = equipment[1].ToString();
            //row 4
            _TEXT_12lbCannons_ONLOAD.text = (onLoadArmory[19] - equipment[2]).ToString();
            _TEXT_12lbCannons_EQ.text = equipment[2].ToString();
            //row 5
            _TEXT_24lbCannons_ONLOAD.text = (onLoadArmory[20] - equipment[3]).ToString();
            _TEXT_24lbCannons_EQ.text = equipment[3].ToString();
        }

        private void RefreshEquipmentRow(byte index)
        {
            var onLoadArmory = ShipInventory.onLoad[index];
            var equipment = ShipInventory.equipment[index];
            Text t1, t2;
            switch (index)
            {
                case 0: t1 = _TEXT_Falconets_ONLOAD; t2 = _TEXT_Falconets_EQ; break;
                case 1: t1 = _TEXT_8lbCannons_ONLOAD; t2 = _TEXT_8lbCannons_EQ; break;
                case 2: t1 = _TEXT_12lbCannons_ONLOAD; t2 = _TEXT_12lbCannons_EQ; break;
                case 3: t1 = _TEXT_24lbCannons_ONLOAD; t2 = _TEXT_24lbCannons_EQ; break;
                default: t1 = null; t2 = null; break;

            }
            t1.text = onLoadArmory.ToString();
            t2.text = equipment.ToString();
        }
#endregion

#region MORALE & SUPPLIES
        private void CheckMorale()
        {

            _TEXT_MoralBySupplies.text = (Mathf.Floor(ShipInventory.Morale_Supplies * 100)).ToString() + "%";
            _TEXT_MoralByPillage.text = (Mathf.Floor(ShipInventory.Morale_Pillage * 100)).ToString() + "%";
            _TEXT_MoraleByRest.text = (Mathf.Floor(ShipInventory.Morale_Resting * 100)).ToString() + "%";

            _FILLER_MoraleSupplies.fillAmount = ShipInventory.Morale_Supplies;
            SetFillerColor(_FILLER_MoraleSupplies);

            _FILLER_MoralePillage.fillAmount = ShipInventory.Morale_Pillage;
            SetFillerColor(_FILLER_MoralePillage);

            _FILLER_Rest.fillAmount = ShipInventory.Morale_Resting;
            SetFillerColor(_FILLER_Rest);

            var mod = MoraleModifier.GetMoraleModifier();
            var global = ShipInventory.Morale_Global + mod;

            //On lower display
            _TEXT_Morale_GLOBAL.text = Mathf.Clamp((Mathf.Floor(global * 100)), 0,100).ToString() + "%";

            //On panel 
            _TEXT_OnPanel_MoraleByModifiers.text = Mathf.Floor(mod * 100).ToString() + "%";
            _TEXT_OnPanel_MoraleByEvents.text = "0%";
            _TEXT_OnPanel_MoraleGLOBAL.text = Mathf.Clamp((Mathf.Floor(global * 100)), 0, 100).ToString() + "%";

            //Set mood icon
            if (global > 0.8f)
            {
                _IMG_Morale.sprite = _moods[0];
            }
            else if (global > 0.6f)
            {
                _IMG_Morale.sprite = _moods[1];
            }
            else if(global > 0.4f)
            {
                _IMG_Morale.sprite = _moods[2];
            }
            else if(global > 0.25f)
            {
                _IMG_Morale.sprite = _moods[3];
            }
            else
            {
                _IMG_Morale.sprite = _moods[4];
            }
        }

        private void SetFillerColor(Image target)
        {
            if (target.fillAmount > 0.75f)
            {
                target.color = Color.green;
            }
            else if (target.fillAmount > 0.5f)
            {
                target.color = Color.yellow;
            }
            else if (target.fillAmount > 0.25f)
            {
                target.color = new Color(1, 0.53f, 0);
            }
            else
            {
                target.color = Color.red;
            }
        }
#endregion

#region 3D VIEWER
        /// <summary>
        /// Get the relative angle between the straight looking line and the target rotation (in degrees)
        /// </summary>
        /// <param name="target">the target ship</param>
        /// <param name="inverse">is the resulting angle negative?</param>
        /// <returns></returns>
        private static float GetRelativeAngle(Transform target)
        {
            //Ángulo entre player y barco observado
            var lookAng = Quaternion.LookRotation(target.position - playerRef.transform.position);

            var targetRot = target.rotation.eulerAngles.y;
            print($"{targetRot} --- {target.rotation.eulerAngles.y}" );

            return target.rotation.eulerAngles.y - lookAng.eulerAngles.y;

        }
        #endregion
    }
}

