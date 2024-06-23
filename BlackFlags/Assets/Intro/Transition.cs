using UnityEngine;
//using UnityEngine.Video;
using System.Collections;

//Modding
using System.IO;
using UnityEngine.Networking;
using GameSettings.Core;

namespace GameMechanics.Intro
{
    public class Transition : MonoBehaviour
    {
        //Parameters
        [SerializeField] protected int index;
        [SerializeField] protected float timer;
        [SerializeField] protected bool searchForMods;

        //References:
        [SerializeField] protected Component player;

        private void Start()
        {
            if (timer != 0)
                Invoke("SafetyDelay", timer); //Dummy delay, Check stopped working :(
        }

        private void SafetyDelay()
        {
            Load();
            //StartCoroutine("Check"); 
        }

        protected virtual void Load()
        {
            if (File.Exists("TempCheckMod.txt"))
            {
                if (searchForMods)
                {
                    GetModDataFromPath();
                    index = 4;
                    SceneManager.sceneAfterAsynLoad = 2;
                    StartCoroutine(TryGetExtendedData());
                }
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(index);
            }
        }

        //protected IEnumerator Check()
        //{
        //    while (!((VideoPlayer)player).isPlaying)
        //        yield return new WaitForEndOfFrame();

        //    Load();
        //}


        //Modding

        private void GetModDataFromPath()
        {
            var reader = new StreamReader("TempCheckMod.txt");
            var directory = reader.ReadLine();
            reader.Close();

            PersistentGameSettings.GetModData(directory);
        }

        private IEnumerator TryGetExtendedData()
        {
            var mod = PersistentGameSettings.currentMod;
            var path = "";

            if (mod.jsonLoading.customBackground)
            {
                //Image route
                path = Path.Combine(mod.ModPath, "Data\\Load\\loading.jpg");

                //Request to local path
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
                
                //Asynchronous response
                yield return request.SendWebRequest();

                //Texture
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                //Tecture2d to Sprite
                var rec = new Rect(0, 0, texture.width, texture.height);
                //Set background
                PersistentGameSettings.currentMod.loadingBackground = Sprite.Create(texture, rec, new Vector2(0, 0), 1);
            }

            
            if (mod.jsonMenu.customMenu)
            {
                //Image route
                path = Path.Combine(PersistentGameSettings.currentMod.ModPath, "Data\\Menu\\menu.jpg");

                //Request to local path
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);

                //Asynchronous response
                yield return request.SendWebRequest();

                //Texture
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                //Tecture2d to Sprite
                var rec = new Rect(0, 0, texture.width, texture.height);

                //Set background
                PersistentGameSettings.currentMod.menuBackground = Sprite.Create(texture, rec, new Vector2(0, 0), 1);
            }

            if(mod.jsonFonts.changeFonts)
            {
                //Fonts route
                path = Path.Combine(mod.ModPath, "Fonts");

                var fonts = new Font[3];
                string[] files = { mod.jsonFonts.mainFont, mod.jsonFonts.secondFond, mod.jsonFonts.alternativeFont };

                for (int i = 0; i < 3; i++)
                {
                    //Request to local path
                    //UnityWebRequest request = UnityWebRequest.Get(Path.Combine(path, files[i]) + ".ttf");

                    fonts[i] = Resources.Load<Font>("Fonts/" + files[i]);
                }

                mod.jsonFonts.GetJsonFonts(fonts);
            }

            //Load Scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        }

        //private Font GetFontFromResource()
        //{
        //    Stream fontStream = this.GetType().Assembly.GetManifestResourceStream("yourfont.ttf");

        //    byte[] fontdata = new byte[fontStream.Length];
        //    fontStream.Read(fontdata, 0, (int)fontStream.Length);
        //    fontStream.Close();

        //    return new Font(fontdata);
        //}
    }
}

