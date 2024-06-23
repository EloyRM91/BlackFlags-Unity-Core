using UnityEngine;
using UnityEngine.UI;
using GameMechanics.Utilities;
using System.IO;

using GameSettings.Core;

namespace GameMechanics.Sound
{
    public class SoundtrackManager<T> : MonoBehaviour where T : SingletonBehaviour<T>, IContainsMusicList
    {
        public static void SetLayoutList(Transform trackListContainer, GameObject Prefab_trackRow, string[] songNames = null)
        {
            T currentManager = SingletonBehaviour<T>.GetInstance();
            var list = currentManager.GetSoundtrack();
            TrackRowScript.activeTracks = 0;
            try
            {
                var pathFile = GetPath<T>();
                print(pathFile);

                StreamReader sr = new StreamReader(pathFile);

                for (int i = 0; i < list.Length; i++)
                {
                    var newTrack = Instantiate(Prefab_trackRow, trackListContainer).transform;
                    newTrack.GetChild(1).GetChild(0).GetComponent<Text>().text = songNames != null ? songNames[i] : list[i].name;

                    //Set toggle value
                    var toggleStatus = sr.ReadLine() == "1";
                    newTrack.GetChild(0).GetComponent<Toggle>().isOn = toggleStatus;
                    if (toggleStatus) TrackRowScript.activeTracks++;
                    else
                    {
                        newTrack.GetChild(1).GetComponent<Button>().interactable = false;
                        newTrack.GetChild(1).GetChild(0).GetComponent<Text>().color = TrackRowScript.DisabledColor;
                    }

                }
                sr.Close();
            }
            catch(System.Exception e)
            {
                foreach (AudioClip track in list)
                {
                    var newTrack = Instantiate(Prefab_trackRow, trackListContainer).transform;
                    newTrack.GetChild(1).GetChild(0).GetComponent<Text>().text = track.ToString();

                    //las pistas comienzan activas
                    TrackRowScript.activeTracks++;
                }
            }
        }

        public static void SetPlayListSettings(Transform trackListContainer)
        {
            var pathFile = GetPath<T>();

            StreamWriter writer = new StreamWriter(pathFile);
            foreach (Transform row in trackListContainer)
            {
                var value = row.GetChild(0).GetComponent<Toggle>().isOn;
                writer.WriteLine(value? 1 : 0);
            }
            writer.Close();
        }

        private static string GetPath<P>()
        {
            var pathFile = "";
            var mod = PersistentGameSettings.currentMod;
            if (mod != null)
            {
                pathFile = mod.ModPath + "/Music/";
            }
            else
            {
#if UNITY_EDITOR
                pathFile = "C:/users/" + System.Environment.UserName + "/OneDrive/Escritorio/TrackSettings/";
#else
                pathFile = Application.dataPath + "/TrackSettings/";
#endif
            }
            pathFile += typeof(P).ToString().Split('.')[2] + "TrackList.txt";
            return pathFile;
        }
    }
}

