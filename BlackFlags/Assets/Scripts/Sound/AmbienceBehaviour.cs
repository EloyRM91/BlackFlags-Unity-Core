using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameMechanics.Utilities;

//Modding
using GameSettings.Core;

namespace GameMechanics.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class AmbienceBehaviour<T> : GameMechanics.Utilities.SingletonBehaviour<T> where T: AmbienceBehaviour<T>, IContainsMusicList
    {
        [Header("As Ambience Controller")]
        //References
        protected AudioSource musicAS, responsiveAS;
        [SerializeField] protected Transform layoutContainer;
        [SerializeField] protected Button resetButton;
        [SerializeField] protected Toggle stopPlaylist, adaptForTEA;
        [SerializeField] protected Slider musicSlider;
        [SerializeField] protected OnHandler handle;

        //Music tracks:
        [SerializeField] protected AudioClip[] _music, _buttonResponses;
        protected string[] _songNames; //array auxiliar para guardar los nombres de los audioclips en el caso de que se carguen desde un mod (el audioclip no guarda el nombre)

        //Playlist controller
        protected byte currentTrack;

        protected virtual void Start()
        {
            //Read & Deserialize
            adaptForTEA.isOn = ResponsiveSound.adapt2TEA;

            //Components
            musicAS = GetComponent<AudioSource>();
            responsiveAS = transform.GetChild(0).GetComponent<AudioSource>();

            //Arrange settings panel layout
            var prefab_TracksRow = Resources.Load<GameObject>("Audio Tracks/row - audio track");

            if(PersistentGameSettings.currentMod!= null)
            {
                SoundtrackManager<T>.SetLayoutList(layoutContainer, prefab_TracksRow, _songNames);
            }
            else
            {
                SoundtrackManager<T>.SetLayoutList(layoutContainer, prefab_TracksRow);
            }

            //Listeners
            resetButton.onClick.AddListener( delegate { ResetPlayListSettings(layoutContainer); });
            stopPlaylist.onValueChanged.AddListener( delegate{ ToggleResponse(stopPlaylist, musicSlider); });
            musicSlider.onValueChanged.AddListener(delegate { stopPlaylist.isOn = musicSlider.value == -30; if (musicSlider.value == -30) StopMusic(); });
            adaptForTEA.onValueChanged.AddListener(delegate { ResponsiveSound.adapt2TEA = adaptForTEA.isOn; });

            //Events
            ResponsiveSound.AudioResponse += PlayButtonSound;
        }

        protected virtual void OnDestroy()
        {
            //Events
            ResponsiveSound.AudioResponse -= PlayButtonSound;
        }

        #region LISTENER EVENTS

        private void ToggleResponse(Toggle toggle, Slider slider)
        {
            if (toggle.isOn)
            {

                if(slider.value != -30)
                {
                    slider.value = -30;
                    StopMusic();
                }
            }
            else
            {
                if(!musicAS.isPlaying)
                {
                    if(!handle.IsOnHandler) slider.value = -10;
                    ResetMusic();
                }
            }
        }

        private void ResetPlayListSettings(Transform container)
        {
            foreach(Transform row in container)
            {
                var toggle = row.GetChild(0).GetComponent<Toggle>();

                if (!toggle.isOn)
                {
                    toggle.isOn = true;
                }
            }
        }

        private void PlayButtonSound(AudioResponse type)
        {
            responsiveAS.clip = _buttonResponses[(int)type];
            responsiveAS.Play();
        }

#endregion

        public void ExitFromCurrentScene()
        {
            //Guarda la configuración de audio antes de salir
            SoundtrackManager<T>.SetPlayListSettings(layoutContainer);
        }

        protected void StartPlayList(int clips)
        {
            if (TrackRowScript.activeTracks > 0 && musicSlider.value != -30)
            {
                currentTrack = (byte)Random.Range(0, clips);
                do
                {
                    NextTrack();
                }
                while (!TrackRowScript.isActiveTrack(layoutContainer, currentTrack));
                SetCurrentPlay();
                StartCoroutine("MusicControl");
            }
            else if (musicSlider.value == -30)
                stopPlaylist.isOn = true;

        }

        protected virtual void GetClip()
        {
            if (TrackRowScript.activeTracks > 0)
            {
                if (TrackRowScript.isActiveTrack(layoutContainer, currentTrack))
                {
                    musicAS.clip = SetCurrentClip();
                    musicAS.Play();
                }
                else
                {
                    NextTrack();
                    GetClip();
                }
            }
        }

        protected virtual void NextTrack()
        {
            if (TrackRowScript.activeTracks > 0)
            {
                currentTrack = (byte)(currentTrack == (_music.Length - 1) ? 0 : currentTrack + 1);
            }
            else
            {
                StopCoroutine("MusicControl");
            }
        }

        protected void SkipTrack()
        {
            if (TrackRowScript.activeTracks > 0)
            {
                NextTrack();
                StopCoroutine("MusicControl");
                StartCoroutine("MusicControl");
            }
            else
            {
                musicAS.Stop();
            }
        }

        protected virtual void PlaySelectedTrack(int index)
        {
            if (stopPlaylist.isOn)
                stopPlaylist.isOn = false;
            currentTrack = (byte)index;
            StopCoroutine("MusicControl");
            StartCoroutine("MusicControl");
        }

        protected virtual IEnumerator MusicControl()
        {
            GetClip();
            SetCurrentPlay();
            yield return new WaitForSecondsRealtime(10);
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                if (!musicAS.isPlaying)
                {
                    yield return new WaitForSecondsRealtime(1);
                    NextTrack();
                    GetClip();
                    SetCurrentPlay();
                }
            }
        }

        protected virtual void SetCurrentPlay()
        {
            TrackRowScript.SetCurrentPlay(currentTrack, layoutContainer);
        }

        protected virtual void StopMusic()
        {
            musicAS.Stop();
            StopCoroutine("MusicControl");
        }

        protected virtual void ResetMusic()
        {
            StartCoroutine("MusicControl");
        }

        public abstract AudioClip[] GetSoundtrack();
        protected abstract AudioClip SetCurrentClip();
    }
}

