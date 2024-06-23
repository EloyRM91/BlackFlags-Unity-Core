using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Modding
using GameSettings.Core;
using UnityEngine.Networking;
using GameMechanics.Sound;

namespace GameSettings.Loading
{
    public class LoadingScript : MonoBehaviour
    {

#region VARIABLES
        //Loading quotations text
        [SerializeField] private Text _TEXT_Loading;
        //Scene Background
        [SerializeField] private Image _background;
        //Quotes Dictionary
        protected Dictionary<byte, string> D_LoadingText = new Dictionary<byte, string>()
        {
            { 0, "El desempeño de la tripulación en combate viene dado por el nivel de moral. No es buena idea iniciar una batalla con la moral muy baja." },
            { 1, "Al conseguir un botín, se repartirá entre la tripulación, aumentando así la moral a bordo. La moral empeorará si transcurre tiempo sin saquear." },
            { 2, "La moral de la tripulación puede aumentarse pasando tiempo en las tabernas, consiguiendo victorias y saqueos, y asegurando las reservas de ron."},
            { 3, "El 19 de septiembre es el “Día Internacional de Hablar como Un Pirata”."},
            { 4, "Todo hombre normal debe tener la tentación, a veces, de escupirse las manos, izar la bandera negra y comenzar a rajar gargantas. - H.L. Mencken"},
            { 5, "Si terminas una frase llamando a alguien “rata de sentina” la gente pensará, “¿Quién es ese tipo tan genial que habla como un pirata? Quiero ser su amigo”."},
            { 6, "“Do What You Want, Cause A Pirate Is Free... You Are A Pirate!” – Lazy Town, Canción Infantil"},
            { 7, "Si escribes yourareapirate.ytmnd.com en el navegador no te vas a arrepentir."},
            { 8, "Con el tiempo, el nivel de amistad con los personajes del mundo se reduce poco a poco. Negocia con contrabandistas y bebe con otros capitanes para mantener el nivel de amistad."},
            { 9, "Algunos refugios de piratas requieren que tu lealtad al código tenga cierto valor para acceder, o que tu reputación sea alta."},
            { 10, "Los piratas solían carenar sus barcos en bahías apartadas porque no tenían acceso a los diques secos. Busca un refugio en el mapa para poder reparar tu embarcación"}
        };
#endregion
        void OnEnable()
        {

            //Use custom data when a mod has been loaded:
            var mod = PersistentGameSettings.currentMod;
            if (mod != null)
            {
                //Background image:
                if (mod.loadingBackground != null)
                {
                    _background.sprite = mod.loadingBackground;
                }

                //Loading text:
                var quotes = mod.jsonLoading.customQuotes;
                if (quotes != null)
                {
                    if (quotes.Length > 0)
                        _TEXT_Loading.text = quotes[(byte)Random.Range(0, quotes.Length)];
                    else
                        _TEXT_Loading.text = D_LoadingText[(byte)Random.Range(0, D_LoadingText.Count)];
                }
                else
                    _TEXT_Loading.text = D_LoadingText[(byte)Random.Range(0, D_LoadingText.Count)];
            }
            else
            {
                _TEXT_Loading.text = D_LoadingText[(byte)Random.Range(0, D_LoadingText.Count)];
            }

            if (mod != null)
            {
                if (mod.jsonTracks.setMenuTracks)
                {
                    if (SceneManager.sceneAfterAsynLoad == 2)
                        StartCoroutine(TryGetMenuAudio(mod));
                }

                if (mod.jsonTracks.setGameTracks)
                {
                    if (SceneManager.sceneAfterAsynLoad == 3)
                        StartCoroutine(TryGetGameAudio(mod));
                }

                if(!mod.jsonTracks.setMenuTracks & !mod.jsonTracks.setGameTracks)
                    Invoke("Load", 0.3f); //Dummy patch
            }
            else
            {
                Invoke("Load", 0.3f); //Dummy patch
            }
        }

        protected void Load()
        {
            try
            {
                PersistentAudioSettings.SaveAudioSettings();
            }
            catch { }

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneManager.sceneAfterAsynLoad);
        }

        IEnumerator TryGetMenuAudio(ModData mod)
        {
            //Files route
            var path = mod.ModPath + "/Music/menu/";

            var jsonSongs = mod.jsonTracks.songsMenu;

            if(jsonSongs.Length > 0)
            {
                Song[] tracks = new Song[jsonSongs.Length];

                for (int i = 0; i < jsonSongs.Length; i++)
                {
                    //Request to local path
                    UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path + jsonSongs[i].trackName + ".mp3", AudioType.MPEG);

                    //Asynchronous response
                    yield return request.SendWebRequest();

                    //Get AudioClip
                    var thisSong = new Song(DownloadHandlerAudioClip.GetContent(request), jsonSongs[i].songName, jsonSongs[i].description);

                    //Add song to songs array
                    tracks[i] = thisSong;
                }

                GameObject persistentContainer = new GameObject();
                persistentContainer.name = "DELIVERY";
                var delivery = persistentContainer.AddComponent<MenuMusicDelivery>();
                delivery.shipmentData = tracks;
            }

            Load();
        }

        IEnumerator TryGetGameAudio(ModData mod)
        {
            //Files route
            var path = mod.ModPath + "/Music/game/";
            
            var jsonSongs = mod.jsonTracks.songsGame;

            if (jsonSongs.Length > 0)
            {
                AudioClip[] tracks = new AudioClip[jsonSongs.Length];

                for (int i = 0; i < jsonSongs.Length; i++)
                {
                    //Request to local path
                    UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path + jsonSongs[i] + ".mp3", AudioType.MPEG);

                    //Asynchronous response
                    yield return request.SendWebRequest();

                    //Get AudioClip
                    var thisSong = DownloadHandlerAudioClip.GetContent(request);

                    //Add song to songs array
                    tracks[i] = thisSong;
                }

                GameObject persistentContainer = new GameObject();
                persistentContainer.name = "DELIVERY";
                var delivery = persistentContainer.AddComponent<GameMusicDelivery>();
                delivery.shipmentData = tracks;
            }

            Load();
        }

        private void CreateDelivery<T, P>(P tracks) where T : Delivery<P>
        {
            //GameObject persistentContainer = GameObject.FindWithTag("Persistent");

            GameObject persistentContainer = new GameObject();
            persistentContainer.name = "DELIVERY";
            var delivery = persistentContainer.AddComponent<T>();
            delivery.shipmentData = tracks;
        }
    }
}

