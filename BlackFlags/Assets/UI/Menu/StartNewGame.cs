using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using GameMechanics.Mechanics;

namespace GameSettings.Core
{
    public class StartNewGame : MonoBehaviour
    {
        //Adaptive flag
        //[SerializeField] private Image _IMG_Flag, _IMG_Background, _IMG_Front;

        //Difficulty selection
        [SerializeField] private Text _TEXT_Gold, _Text_Modifiers, _Text_SelectedDifficulty;
        [SerializeField] private Image _IMG_ResourcesLevel;
        [SerializeField] private Sprite[] _resourceLevels;

        //Toggles
        [SerializeField] private Toggle _TOGGLE_MoreEvents, _TOGGLE_AggressiveKingdoms, _TOGGLE_AlwaysAttack;

        //Flag View
        [SerializeField] private GameObject[] _flagScenes;
        [SerializeField] private GameObject _flagViewDetails;
        [SerializeField] private GameObject _flagSelectBackground;
        [SerializeField] private PostProcessLayer _ppLayer;
        [SerializeField] private WavingEffect _wavingEffect;
        private Vector3 _cameraOffset;
        [SerializeField] private Material _waterMat;
        private bool activatedBefore = false;

        void Awake()
        {
            _cameraOffset = _wavingEffect.transform.position;
        }

        void OnEnable()
        {
            _wavingEffect.transform.position = _cameraOffset;
            _flagSelectBackground.SetActive(false);
            _flagViewDetails.SetActive(true);

            try
            {
                transform.GetChild(3).GetChild((int)PersistentGameData._GData_Difficulty).GetComponent<Button>().Select();
            }
            catch
            {
                //OwO
            }

            if(activatedBefore)
            {
                _flagScenes[(int)PersistentGameData._GData_Difficulty - 1].SetActive(true);
            }
        }

        void OnDisable()
        {
            try
            {
                _flagSelectBackground.SetActive(true);
                _flagViewDetails.SetActive(false);

                for (int i = 0; i < _flagScenes.Length; i++)
                {
                    _flagScenes[i].SetActive(false);
                }
            }
            catch
            { }
            _waterMat.SetFloat("_Darkness", 0.085f);
        }


        void Start()
        {
            activatedBefore = true;
            for (int i = 1; i < 5; i++)
            {
                var level = (GameDifficulty)i;
                transform.GetChild(3).GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { SelectDifficulty(level); });
            }

            transform.GetChild(3).GetChild(2).GetComponent<Button>().Select();
            SelectDifficulty(GameDifficulty.normal);

            _TOGGLE_MoreEvents.onValueChanged.AddListener(delegate { PersistentGameData._GDATA_MoreEvents = _TOGGLE_MoreEvents.isOn; });
            _TOGGLE_AggressiveKingdoms.onValueChanged.AddListener(delegate { PersistentGameData._GDATA_AgressiveKingdoms = _TOGGLE_AggressiveKingdoms.isOn; });
            _TOGGLE_AlwaysAttack.onValueChanged.AddListener(delegate { PersistentGameData._GDATA_AgressiveKingdoms = _TOGGLE_AggressiveKingdoms.isOn; });
        }

        private void SelectDifficulty(GameDifficulty level)
        {
            PersistentGameData._GData_Difficulty = level;

            if(level == GameDifficulty.nightmare)
            {
                _TOGGLE_AlwaysAttack.isOn = true;
                _TOGGLE_AlwaysAttack.interactable = false;
            }
            else
            {
                _TOGGLE_AlwaysAttack.interactable = true;
            }

            _TEXT_Gold.text = $"inicial: {PersistentGameData._GData_Gold}";
            _Text_SelectedDifficulty.text = "Dificultad: ";
            switch (level)
            {
                case GameDifficulty.easy:
                    //_TEXT_Gold.text += 1100;
                    _Text_SelectedDifficulty.text += "Fácil";
                    _wavingEffect.amplitude = 0.1f;
                    break;
                case GameDifficulty.normal:
                    //_TEXT_Gold.text += 750;
                    _Text_SelectedDifficulty.text += "Normal";
                    _wavingEffect.amplitude = 0.15f;
                    break;
                case GameDifficulty.hard:
                    //_TEXT_Gold.text += 450;
                    _Text_SelectedDifficulty.text += "Difícil";
                    _wavingEffect.amplitude = 0.25f;
                    break;
                case GameDifficulty.nightmare:
                    //_TEXT_Gold.text += 300;
                    _Text_SelectedDifficulty.text += "Pesadilla";
                    _wavingEffect.amplitude = 0.15f;
                    break;
            }

            _IMG_ResourcesLevel.sprite = _resourceLevels[(int)level - 1];

            _wavingEffect.SetMovement();

            if (level != GameDifficulty.nightmare)
            {
                _Text_Modifiers.text =
                    $"Dificultad para aumentar la amistad con personajes: {(PersistentGameData._GData_FriendshipModier >= 0 ? "+" : "")}{PersistentGameData._GData_FriendshipModier * 100}%\n" +
                    $"Beneficio por contrabando: {(PersistentGameData._GData_TradeModifier >= 0 ? "+" : "")}{PersistentGameData._GData_TradeModifier * 100}%\n" +
                    $"Actividad de patrullas y corsarios: {(level == GameDifficulty.easy ? "baja" : level == GameDifficulty.normal ? "normal" : "alta")}\n" +
                    $"Moral de la tripulación: {(PersistentGameData._GData_MoraleModifier >= 0 ? "+" : "")}{PersistentGameData._GData_MoraleModifier * 100}%";
            }
            else
            {
                _Text_Modifiers.text = "En modo \"Pesadilla\" la dificultad de comerciar, entablar amistades en el bajo mundo y satisfacer la vida a bordo será máxima, y además la moral a bordo no puede superar el 90% y las patrullas y corsarios siempre atacan.";
            }

            //Vista de dificultad

            for (int i = 0; i < _flagScenes.Length; i++)
            {
                _flagScenes[i].SetActive(false);
            }

            _flagScenes[(int)level - 1].SetActive(true);
            _ppLayer.enabled = level == GameDifficulty.nightmare;
        }
    }
}


