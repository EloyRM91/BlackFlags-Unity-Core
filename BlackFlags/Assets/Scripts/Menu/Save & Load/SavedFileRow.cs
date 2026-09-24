using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.WorldMap;

namespace UI.Files
{
    public class SavedFileRow : MonoBehaviour
    {
        [SerializeField] private Text _TEXT_version;
        [SerializeField] private Text _TEXT_fileName;
        [SerializeField] private Text _TEXT_fileLocation;
        [SerializeField] Image _IMG_LocIcon;
        [SerializeField] private Text _TEXT_gameDate;
        [SerializeField] private Text _TEXT_fileDate;
        [SerializeField] private Text _TEXT_fileSize;
        [SerializeField] Image _IMG_Flag;
        [SerializeField] Image _IMG_WarningSprite;

        [SerializeField]
        private SpriteCatalog spriteCatalog;

        //Events
        public delegate void Selection(SavedFileRow row);
        public static event Selection SelectFile;

        private byte errorCause;
        /**
        0 - ok
        1 - no meta
        2 - old version
        **/

        private static Dictionary<byte, string> _D_ErrorMsgs = new Dictionary<byte, string>() {
            {1, "No se encontró el archivo meta asociado a la partida"},
            {2, "La partida fue creada en una versión distinta"}
        };

        //todo: esto está muy harcodeado, ya que requiero directamente el path
        //todo: crear un scriptable object
        // private static Dictionary<string, string> _D_Sprites = new Dictionary<string, string>()
        // {
        //     {"onSail", "UI/Icons/icon - rudder"},
        //     {"onPort", "UI/Icons/icon - on port"}
        // };

        void Start()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(() => OnFileSelected());
        }

        public string GetFileName()
        {
            return _TEXT_fileName.text.Split('.')[0];
        }

        public void SetData(SavedFileInfo file, SavedMetaInfo meta)
        {
            _TEXT_fileName.text = file.name;
            _TEXT_fileDate.text = file.modifiedDate;
            _TEXT_fileSize.text = file.size;

            if (meta != null)
            {
                //Comprueba la versión
                _TEXT_version.text = meta.version;
                var currentVersion = $"v{Application.version}";

                if (currentVersion != meta.version)
                {
                    SetError(2);
                    _IMG_WarningSprite.enabled = true;
                }

                _TEXT_fileLocation.text = meta.location;

                //!muy harcodeado
                //todo: arreglar esto
                var key = meta.location == "En el mar" ? "OnSail" : "OnPort";
                _IMG_LocIcon.sprite = spriteCatalog.Get(key);


                //Crea el sprite de la bandera sampleada:
                byte[] flagMeta = meta.bytes;
                Sprite sp = GetSpriteFromBytes(flagMeta, 200, 133);

                _IMG_Flag.sprite = sp;

                _TEXT_gameDate.text = meta.gameDate;
            }
            else
            {
                // Debug.Log(file.name + " has no meta");
                if (_IMG_WarningSprite == null)
                {
                    Debug.LogError("_IMG_WarningSprite es NULL");
                }
                SetError(1);
            }

            _IMG_WarningSprite.enabled = errorCause != 0;
        }

        public byte getErrorCause()
        {
            return errorCause;
        }

        private void SetError(byte code)
        {
            errorCause = code;
            var ButtonInfo = _IMG_WarningSprite.transform.GetComponent<ButtonInfo>();
            var message = _D_ErrorMsgs[code];
            ButtonInfo.txt = message;
        }

        private Sprite GetSpriteFromBytes(byte[] bytes, ushort witdh, ushort height)
        {
            // var flagTex = new Texture2D(witdh, height);
            // flagTex.LoadRawTextureData(bytes);
            // flagTex.Apply();
            // return Sprite.Create(flagTex, new Rect(0, 0, witdh, height), new Vector2(0, 0), 1);

            var flagTex = new Texture2D(2, 2);
            flagTex.LoadImage(bytes);
            return Sprite.Create(flagTex, new Rect(0, 0, flagTex.width, flagTex.height), new Vector2(0.5f, 0.5f), 100);
        }

        private void OnFileSelected()
        {
            SelectFile(this);
            GetComponent<Image>().enabled = true;
        }

        public void UnselectThisRow()
        {
            GetComponent<Image>().enabled = false;
        }
    }
}

