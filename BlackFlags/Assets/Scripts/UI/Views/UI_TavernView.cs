using UnityEngine;

//Mechanics:
using GameMechanics.WorldCities;
using GameMechanics.Data;

//UI
using UnityEngine.UI;
using UI.WorldMap;

//Tweening
using DG.Tweening;

//System
using System.Collections;


public class UI_TavernView : UI_ScenicView<UI_TavernView>
{
    //instance
    //private static UI_TavernView instance;

    [Header("CHARACTERS LIST")]
    //Characters list's layout display
    [SerializeField] private Transform _LayoutContainer;
    [SerializeField] private GameObject _PREFAB_row;

    [Header("CHARACTER HEADER DATA")]
    //Tavern-view's ui label panel info
    [SerializeField] private Text _TEXT_CharacterLabel;
    [SerializeField] private Text _TEXT_RoleLabel;
    [SerializeField] private Text _TEXT_friendshipLevel;
    [SerializeField] private Image _IMG_CharacterRoleIcon;
    //Instantiating attributes
    [SerializeField] private Sprite[] _IMGS_Attributes;
    [SerializeField] private Transform _content_ATTRIBUTES;
    [SerializeField] private GameObject _PREFAB_icon_ATTRIBUTE;
    [SerializeField]
    private Text _TEXT_AttributesTargetText;
    //Friendship
    [SerializeField] private Sprite[] _IMGS_Friendship;
    [SerializeField] private Image _IMG_FriendshipIcon;

    [Header("CHARACTER GRAPHICS")]
    [SerializeField] private Layer[] _CharacterLayers;
    [SerializeField] private Layer[] _CharacterLayersBIG;
    [SerializeField] private Sprite[] _closedEyes;
    [SerializeField] private Image _SampleSmuggler;
    [SerializeField] private GameObject _CharacterPirate;

    [Header("TAVERN DATA (PLACE)")]
    //TextLabel
    [SerializeField] private Text _TEXT_Label;
    [SerializeField] private Text _TEXT_PlayerGold;
    //Current View Data
    private MB_City currentPort;

    //private void Awake()
    //{
    //    instance = this;
    //}

    #region EVENTS

    //private void Start()
    //{
    //    PersistentGameData.updateGold += SetGoldText;
    //}
    //private void OnDestroy()
    //{
    //    PersistentGameData.updateGold -= SetGoldText;
    //}

    protected override void SetGoldText(int val)
    {
        _TEXT_PlayerGold.text = val.ToString();
    }

    #endregion

    private void OnEnable()
    {
        //Get this Port Data
        currentPort = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().GetPort();

        //Set Tavern name
        _TEXT_Label.text = $"Taberna \"{currentPort.tavernName}\", {currentPort.cityName}";

        //Currency
        SetGoldText(PersistentGameData._GData_Gold);

        //Characters List
        var characters = currentPort.GetCharacters(true);

        while (_LayoutContainer.childCount != 0)
        {
            DestroyImmediate(_LayoutContainer.GetChild(_LayoutContainer.childCount - 1).gameObject);
        }

        for (int i = 0; i < characters.Count; i++)
        {
            var character = characters[i];
            var ch = Instantiate(_PREFAB_row, _LayoutContainer).transform;
            ch.GetChild(0).GetComponent<Text>().text = character.GetCharacterName();
            ch.GetChild(1).GetComponent<Text>().text = character.GetRoleName();
            ch.GetComponent<RowCharacterButton>().character = character;
        }
    }

    public static void SetCharacterLabelST(Character character)
    {
        instance.SetCharacterLabel(character);
        instance.SetCharacterImage(character);
    }

    private void SetCharacterLabel(Character character)
    {
        //Clear attributes display
        while (_content_ATTRIBUTES.childCount != 0)
        {
            DestroyImmediate(_content_ATTRIBUTES.GetChild(0).gameObject);
        }

        //Character's Data
        _TEXT_CharacterLabel.text = character.CharacterName;
        _TEXT_RoleLabel.text = character.GetRoleName();

        //Icon's sprite
        if (character is Smuggler) _IMG_CharacterRoleIcon.sprite = UIMap.ui.GetFlag(6);
        else _IMG_CharacterRoleIcon.sprite = UIMap.ui.GetFlag(7);

        //Attributes
        if (character is Pirate)
        {
            var cPirate = (Pirate)character;
            for (int i = 0; i < cPirate.attributes.Length; i++)
            {
                AttributeInfo atInfo = Instantiate(_PREFAB_icon_ATTRIBUTE, _content_ATTRIBUTES).GetComponent<AttributeInfo>();
                atInfo.attribute = cPirate.attributes[i];
                atInfo.GetComponent<Image>().sprite = _IMGS_Attributes[(int)atInfo.attribute.attribute];
                atInfo.SetTextTarget(_TEXT_AttributesTargetText);
            }
        }

        //Friendship Level
        SetFriendshipLevel(character);
    }

    public static void SetFriendshipLevelST(Character character)
    {
        instance.SetFriendshipLevel(character);
    }

    private void SetFriendshipLevel(Character character)
    {
        float val = character.FriendshipLevel * 100;
        float rounded = (int)(val * 10) * 0.1f;
        _TEXT_friendshipLevel.text = rounded + "%";
        _IMG_FriendshipIcon.sprite = GetFriendshipIcon(character.FriendshipLevel);
    }

    private void SetCharacterImage(Character character)
    {
        if(character is Pirate)
        {
            _CharacterPirate.SetActive(true);

            var pirate = (Pirate)character;
            var layers = pirate.big ? _CharacterLayersBIG : _CharacterLayers;

            //Get character custom seed
            int[] seeds = pirate.GetSeed();
            for (int i = 0; i < layers.Length; i++)
            {

                //set character's sprites
                Layer layer = layers[i];
                layer.layerImage.sprite = layer.layerSprites[seeds[i]];

                //Fade
                layer.layerImage.DOFade(0, 0);
                if(layer.layerImage.sprite != null)
                    layer.layerImage.DOFade(1, .6f);
            }

            //Blink
            StopAllCoroutines();
            StartCoroutine(Blink(layers[6], seeds[6]));
        }
        else
        {
            _SampleSmuggler.gameObject.SetActive(true);
            _SampleSmuggler.DOFade(0, 0);
            _SampleSmuggler.DOFade(1, .6f);
        }


    }

    private IEnumerator Blink(Layer faceLayer, int i)
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(0,4) == 0 ? 0.15f : Random.Range(4,7));
            faceLayer.layerImage.sprite = _closedEyes[i];
            yield return new WaitForSeconds(0.15f);
            faceLayer.layerImage.sprite = faceLayer.layerSprites[i];
        }
    }
    private Sprite GetFriendshipIcon(float level)
    {
        int index;
        if (level < 0.2f)
            index = 0;
        else if (level < 0.55f)
            index = 1;
        else if (level < 0.75f)
            index = 2;
        else index = 3;
        return _IMGS_Friendship[index];
    }

    [System.Serializable]
    public class Layer 
    {
        public Image layerImage;
        public Sprite[] layerSprites;
    }
}
