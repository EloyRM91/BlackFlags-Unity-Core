using UnityEngine;

//Reading/Writing
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UI;

//Mods
using GameSettings.Mods;

namespace GameSettings.Core
{
    public class PersistentGameSettings : PersistentRaptor
    {
        #region VARIABLES
        //Reading/Writing path
        private string path = "GameSettings.txt";

        //Settings
        public static bool
            _playTutorial, //�Preguntar al jugador si desea jugar a la tutorial antes de iniciar partida o campa�a?
            _advisorPanel, //�Mostrar consejos y ayudas durante el juego?
            _autoSkipDialog, //�Saltar di�logos en secuencia autom�ticamente?
            _showDialogs; //�Mostrar di�logos?

        //Mods
        public static ModData currentMod = null;
        #endregion
        protected override void Awake()
        {
            if (currentMod != null)
            {
                path = currentMod.ModPath + "/Data/ModSettings.txt";

                if (!currentMod.jsonFonts.changeFonts)
                    GetFonts();
            }
            else
            {  
                GetFonts();
            }
            ReadSettings();
        }
        private void OnDisable()
        {
            WriteSettings();
        }

        //Reading / Writing
        private void ReadSettings()
        {
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                int i = 0;

                while (!reader.EndOfStream)
                {
                    string[] substrings = reader.ReadLine().Split(' ');
                    var val = substrings[1];
                    switch (i)
                    {
                        case 0: _playTutorial = val == "True"; break;
                        case 1: _advisorPanel = val == "True"; break;
                        case 2: _autoSkipDialog = val == "True"; break;
                        case 3: _showDialogs = val == "True"; break;
                        case 4: GameMechanics.Sound.ResponsiveSound.adapt2TEA = val == "True"; break;
                    }
                    i++;
                }

                reader.Close();
            }
        }

        private void WriteSettings()
        {
            FileStream fs = File.Create(path);
            string txtFile =
                $"SuggestTutorial {_playTutorial}" +
                $"\nShowAdvisorPanel {_advisorPanel}" +
                $"\nAutoSkipDialog {_autoSkipDialog}" +
                $"\nShowDialog {_showDialogs}" +
                $"\nAdaptForTEA {GameMechanics.Sound.ResponsiveSound.adapt2TEA}";
            byte[] info = new UTF8Encoding(true).GetBytes(txtFile);
            fs.Write(info, 0, info.Length);
            fs.Close();
        }

        private void GetFonts()
        {
            var p = Resources.Load<Font>("Fonts/PiratesBay");
            var c = Resources.Load<Font>("Fonts/Caligraf 1435");
            var r = Resources.GetBuiltinResource<Font>("Arial.ttf");

            //GameText.Pirate = p;
            //GameText.Caligraf = c;
            //GameText.Regular = r;

            GameText._D_FontStyles = new Dictionary<FontType, UI.FontStyle[]>
            {
                {FontType.mainTitle, new UI.FontStyle[] { new UI.FontStyle(p, 90), new UI.FontStyle(p, 95), new UI.FontStyle(r, 80) } },
                {FontType.mainMenuButton, new UI.FontStyle[] { new UI.FontStyle(p, 50), new UI.FontStyle(p, 55), new UI.FontStyle(r, 42) } },
                {FontType.subButton, new UI.FontStyle[] { new UI.FontStyle(p, 40), new UI.FontStyle(p, 45), new UI.FontStyle(r, 35) } },
                {FontType.text22, new UI.FontStyle[] { new UI.FontStyle(p, 22), new UI.FontStyle(p, 26), new UI.FontStyle(r, 21) } },
                {FontType.text27, new UI.FontStyle[] { new UI.FontStyle(p, 27), new UI.FontStyle(p, 30), new UI.FontStyle(r, 26) } },
                {FontType.title, new UI.FontStyle[] { new UI.FontStyle(p, 67), new UI.FontStyle(p, 75), new UI.FontStyle(r, 60) } },
                {FontType.caligraf, new UI.FontStyle[] { new UI.FontStyle(c, 35), new UI.FontStyle(c, 37), new UI.FontStyle(r, 26) } },
            };

            GameText.canUpdate = true;
        }

        //Modding
        public static void GetModData(string directory)
        {
            currentMod = new ModData(directory);

            ModData.GetJsonFile(File.ReadAllText(Path.Combine(directory, "Data/Load/loading.json")), out currentMod.jsonLoading);
            ModData.GetJsonFile(File.ReadAllText(Path.Combine(directory, "Data/Menu/menu.json")), out currentMod.jsonMenu);
            ModData.GetJsonFile(File.ReadAllText(Path.Combine(directory, "Fonts/styles.json")), out currentMod.jsonFonts);
            ModData.GetJsonFile(File.ReadAllText(Path.Combine(directory, "Music/tracks.json")), out currentMod.jsonTracks);
            ModData.GetJsonFile(File.ReadAllText(Path.Combine(directory, "Data/gameData.json")), out currentMod.gameLogic);
        }

    }

    public class ModData
    {
        //public ModData currentMod;
        private string 
            modName,
            modVersion,
            modReadme,
            modPath,
            modStreaming;

        public string ModName { get { return modName; } }
        public string ModPath { get { return modPath; } }
        public string ModVersion { get { return modVersion; } }
        public string ModReadme { get { return modReadme; } }
        public string ModStreaming { get { return modStreaming; } }

        //DDBB
        //public string ShipNamesPath { get { return modStreaming + "DB_ShipNames.db"; } }
        //public string CharactersNamesPath { get { return modStreaming + "DB_CharacterNames.db"; } }
        //public string ShipSpawnPath { get { return modStreaming + "DB_ShipSpawns.db"; } }

        //Media
        public Sprite
            loadingBackground,
            menuBackground;

        //Json files
        public JSON_Loading jsonLoading;
        public JSON_Menu jsonMenu;
        public JSON_Fonts jsonFonts;
        public JSON_Tracks jsonTracks;
        public JSON_GameData gameLogic;

        public ModData(string directory)
        {
            //modPath = directory;
            var path = directory + "/version.txt";
            
            StreamReader reader;
            if (File.Exists(path))
            {
                reader = new StreamReader(path);
                modName = reader.ReadLine();
                modVersion = reader.ReadLine();
                reader.Close();
            }
            else
            {
                modVersion = "versi�n: ??????";
            }

            path = directory + "readme.txt";
            if (File.Exists(path))
            {
                reader = new StreamReader(path);
                modReadme = reader.ReadLine();
                reader.Close();
            }
            modPath = directory;
            modStreaming = Path.Combine(directory, "StreamingAssets/");
        }

        public static void GetJsonFile<T>(string path, out T file) where T : JSON_MOD
        {
            file = JSON_MOD.GetJson<T>(path);
        }
    }

    public enum GameDifficulty { easy = 1, normal = 2, hard = 3, nightmare = 4}
}

namespace GameSettings.Mods
{
    public abstract class JSON_MOD
    {
        public static T GetJson<T>(string file) where T : JSON_MOD
        {
            return JsonUtility.FromJson<T>(file);
        }
    }

    public class JSON_Loading : JSON_MOD
    {
        public bool
            showPhrases,
            customBackground;
        public string[] customQuotes;

        public string GetSample()
        {
            return customQuotes[(byte)Random.Range(0, customQuotes.Length)];
        }

        public JSON_Loading(string[] quotes, bool show = true, bool setBackground = false)
        {
            showPhrases = show;
            customBackground = setBackground;
            customQuotes = quotes;
        }
    }

    public class JSON_Menu : JSON_MOD
    {
        public bool showFlag, customMenu;
        public float
            cameraDistance,
            opacity,
            xOffset,
            yOffset,
            zOffset,
            xReScaled,
            yRescaled,
            zRescaled;

        public void SetTransform(Transform bottom)
        {
            bottom.position += new Vector3(xOffset, yOffset, zOffset);
            bottom.localScale = new Vector3(xReScaled, yRescaled, zRescaled);
        }

        public JSON_Menu(bool showFlag = true, float cameraDistance = 0, float opacity = 1, float xOffset = 0, float yOffset = 0, float zOffset = 0, float xReScaled = 1, float yRescaled = 1, float zRescaled = 1)
        {
            this.showFlag = showFlag;
            this.cameraDistance = cameraDistance;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.zOffset = zOffset;
            this.xReScaled = xReScaled;
            this.yRescaled = yRescaled;
            this.zRescaled = zRescaled;
        }
    }

    public class JSON_Fonts : JSON_MOD
    {
        public bool changeFonts;
        public string
            mainFont,
            secondFond,
            alternativeFont;
        public FontStylesGroup[] groups;

        public JSON_Fonts(string mainFont, string secondFond, string alternativeFont, FontStylesGroup[] groups, bool changeFonts = false)
        {
            this.changeFonts = changeFonts;
            this.mainFont = mainFont;
            this.secondFond = secondFond;
            this.alternativeFont = alternativeFont;
            this.groups = groups;
        }

        public JSON_Fonts()
        {
            changeFonts = false;
        }

        public void GetJsonFonts(Font[] fonts)
        {
            GameText._D_FontStyles = new Dictionary<FontType, UI.FontStyle[]>();
            for (int i = 0; i < 7; i++)
            {
                var group = groups[i];
                GameText._D_FontStyles.Add((FontType)i,  new UI.FontStyle[] { new UI.FontStyle(fonts[groups[i].data[0].data[0]], groups[i].data[0].data[1]), new UI.FontStyle(fonts[groups[i].data[1].data[0]], groups[i].data[1].data[1]), new UI.FontStyle(fonts[groups[i].data[2].data[0]], groups[i].data[2].data[1]) });
            }
        }
    }

    [System.Serializable]
    public class JSON_Tracks : JSON_MOD
    {
        public bool setMenuTracks, setGameTracks;
        public SerializableSong[] songsMenu;
        public string[] songsGame;

        public JSON_Tracks(SerializableSong[] songsMenu, string[] songsGame, bool setMenuTracks = false, bool setGameTracks = false)
        {
            this.setMenuTracks = setMenuTracks;
            this.setGameTracks = setGameTracks;
            this.songsMenu = songsMenu;
            this.songsGame = songsGame;
        }

        public JSON_Tracks()
        {
            setMenuTracks = false;
            setGameTracks = false;
        }
    }

    /// <summary>
    /// Clase donde se almancela cualquier cambio en la lógica de juego del mod
    /// </summary>
    public class JSON_GameData : JSON_MOD
    {
        public string modFileExt = ".mod";

        public JSON_GameData(string modFileExt)
        {
            this.modFileExt = modFileExt;
        }
    }

    [System.Serializable]
    public class SerializableFontStyle
    {
        public int[] data = new int[2];
        public SerializableFontStyle(int[] data)
        {
            this.data = data;
        }

        public SerializableFontStyle(int fontIndex, int size)
        {
            this.data = new int[2] { fontIndex, size };
        }
    }

    [System.Serializable]
    public class FontStylesGroup
    {
        public SerializableFontStyle[] data = new SerializableFontStyle[3];

        public FontStylesGroup(SerializableFontStyle[] data)
        {
            this.data = data;
        }
    }

    [System.Serializable]
    public class SerializableSong
    {
        public string
            trackName,
            songName,
            description;

        public SerializableSong(string trackName, string songName, string description)
        {
            this.trackName = trackName;
            this.songName = songName;
            this.description = description;
        }
    }
}

