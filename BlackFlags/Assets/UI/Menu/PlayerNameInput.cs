using UnityEngine;
using UnityEngine.UI;

//Game Data
using Generation.Generators;
using GameMechanics.Data;

//Mechanics
using GameMechanics.Menu;

public class PlayerNameInput : MonoBehaviour
{
    #region VARIABLES
    //Selection
    private int _currentSel = 4;

    //Button's Actions
    //--- Selection
    [SerializeField] private Sprite[] _imgs;
    [SerializeField] private Image _currentSelection, _shipNationSelection, _resumeNationSelection;
    [SerializeField] private Transform _selectionBox;
    [SerializeField] private Transform[] positions;
    [SerializeField] private GameObject _selectionPanel;
    //--- Layout Display On/Off
    [SerializeField] private Sprite[] _buttonSprites;

    //Inputfield
    private InputField _inputField;
    [SerializeField] private Text _resumeText;

    //Name refs
    [SerializeField] private Text n1;
    #endregion
    void Start()
    {
        _inputField = GetComponent<InputField>();
        _inputField.onValueChanged.AddListener(delegate { SetName(); });

        //Generator
        //WorldGenerator.GetCharacterNamesLists();
        WorldGenerator.Initialize();
        _inputField.text = WorldGenerator.GetCharacterName(EntityType_KINGDOM.KINGDOM_Britain);
    }

    public void Select(int val)
    {
        if(val != _currentSel)
        {
            _currentSel = val;
            _selectionBox.position = positions[_currentSel].position;
            _currentSelection.sprite = _imgs[_currentSel];
            _shipNationSelection.sprite = _imgs[_currentSel];
            _resumeNationSelection.sprite = _imgs[_currentSel];
            transform.GetChild(2).GetComponent<Image>().sprite = _buttonSprites[1];
            GetRandomName();
        }
    }

    public void SelectLayOut()
    {
        _selectionPanel.SetActive(!_selectionPanel.activeSelf);
        transform.GetChild(2).GetComponent<Image>().sprite = _selectionPanel.activeSelf ? _buttonSprites[0] : _buttonSprites[1];
    }
    private void SetName()
    {
        n1.text = "Bandera Negra de " + _inputField.text;
        _resumeText.text = _inputField.text;
        PersistentGameData._GData_PlayerName = _inputField.text;
        PersistentGameData._GDataPlayerNation = (EntityType_KINGDOM)_currentSel;
    }
    public void GetRandomName()
    {
        _inputField.text = WorldGenerator.GetCharacterName((EntityType_KINGDOM)_currentSel);
    }
}
