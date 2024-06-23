using System.Collections;
using UnityEngine;
//Mechanics
using GameMechanics.WorldCities;
//Tweening
using DG.Tweening;
//Modding
using GameSettings.Core;

namespace GameMechanics.Sound
{

    public class Ambience : AmbienceBehaviour<Ambience>, IContainsMusicList
    {
        [Header("As Ingame Sound Controller")]
        //Parameters
        [SerializeField] private float _offset;
        [SerializeField] [Range(0, 10)] private float _ratio;

        //Data
        [SerializeField]
        private AudioClip[]
            _ambientSound_SHIPS,
            _ambientSound_TAVERN,
            _sails, _pause,
            _bells,
            _cityArrival,
            _villageArrival,
            _naturalPortArrival,
            _exitPort;
        
        //Reference
        private AudioSource responsiveAS2, ambienceAS;


        protected override void Start()
        {

            //Modding
            if (PersistentGameSettings.currentMod != null)
            {
                var delivery = GameObject.FindWithTag("Delivery").GetComponent<GameMusicDelivery>();

                _music = delivery.Deliver();
                _songNames = PersistentGameSettings.currentMod.jsonTracks.songsGame;
            }
            base.Start();

            //Components
            responsiveAS2 = transform.GetChild(1).GetComponent<AudioSource>();
            ambienceAS = transform.GetChild(2).GetComponent<AudioSource>();

            //Events
            PlayerMovement._EVENT_ArriveToPort += SetArrivalSound;
            PlayerMovement._EVENT_ExitFromPort += SetExitPortSound;
            TrackRowScript.SelectTrack += PlaySelectedTrack;
            TrackRowScript.SkipCurrent += SkipTrack;

            //Music
            //Start Music playlist
            StartPlayList(_music.Length);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerMovement._EVENT_ArriveToPort -= SetArrivalSound;
            PlayerMovement._EVENT_ExitFromPort -= SetExitPortSound;
            TrackRowScript.SelectTrack -= PlaySelectedTrack;
            TrackRowScript.SkipCurrent -= SkipTrack;
        }

        IEnumerator AmbienceControl(AmbientType ambient)
        {
            yield return new WaitForSeconds(_offset);
            GetAmbientSoundClip(ambient);
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                if (!ambienceAS.isPlaying)
                {
                    yield return new WaitForSecondsRealtime(Random.Range(0, _ratio));
                    GetAmbientSoundClip(ambient);
                }
            }
        }
        #region RESPONSIVE SOUNDS
        public static void SelectionSails_ST()
        {
            instance.SelectionSails();
        }

        private void SelectionSails()
        {
            responsiveAS.clip = _sails[Random.Range(0, _sails.Length)];
            responsiveAS.Play();
        }
        /// <summary>
        /// Play a clock-ticking pause sound. True: pause ON, false: pause OFF.
        /// </summary>
        /// <param name="val"></param>
        public static void Pause_ST(int val)
        {
            instance.Pause(val);
        }
        private void Pause(int val)
        {
            responsiveAS.clip = _pause[val];
            responsiveAS.Play();
        }
        private void CityArrivalSound()
        {
            responsiveAS2.clip = _cityArrival[Random.Range(0, _cityArrival.Length)];
            responsiveAS2.Play();
        }
        private void VillageArrivalSound()
        {
            responsiveAS2.clip = _villageArrival[Random.Range(0, _villageArrival.Length)];
            responsiveAS2.Play();
        }
        private void NaturalShelterSound()
        {
            responsiveAS2.clip = _naturalPortArrival[Random.Range(0, _naturalPortArrival.Length)];
            responsiveAS2.Play();
        }
        private void ExitSound()
        {
            responsiveAS2.clip = _exitPort[Random.Range(0, _exitPort.Length)];
            responsiveAS2.Play();
        }
        private void SetArrivalSound(KeyPoint k)
        {
            if (k is MB_City)
            {
                CityArrivalSound();
            }
            else if (k is MB_Town)
            {
                VillageArrivalSound();
            }
            else if (k is MB_NaturalPort || k is MB_SmugglersPost)
            {
                NaturalShelterSound();
            }
        }
        private void SetExitPortSound()
        {
            ExitSound();
        }
        #endregion

        #region OVERRIDES - MUSIC
        public override AudioClip[] GetSoundtrack()
        {
            return _music;
        }

        protected override AudioClip SetCurrentClip()
        {
            return _music[currentTrack];
        }
        #endregion

        #region AMBIENCE SOUND
        public static void TransitionToAmbience(bool val, AmbientType ambientType)
        {
            instance.musicAS.DOFade(val ? 0 : 1, 2).SetUpdate(true);
            instance.ambienceAS.DOFade(val ? 1 : 0, 2).SetUpdate(true);
            instance.SetAudioMode(val, ambientType);

        }
        private void SetAudioMode(bool ToAmbience, AmbientType ambient)
        {
            if (ToAmbience) StartCoroutine(AmbienceControl(ambient));
            else
            {
                StopCoroutine(AmbienceControl(ambient));
                Invoke("StopAmbience", 2);
            }
        }

        private void GetAmbientSoundClip(AmbientType ambience)
        {
            switch (ambience)
            {
                case AmbientType.OnSea:
                    ambienceAS.clip = _ambientSound_SHIPS[Random.Range(0, _ambientSound_SHIPS.Length)];
                    break;
                case AmbientType.OnTavern:
                    ambienceAS.clip = _ambientSound_TAVERN[Random.Range(0, _ambientSound_TAVERN.Length)];
                    break;
                case AmbientType.OnCity:
                    ambienceAS.clip = _ambientSound_SHIPS[Random.Range(0, _ambientSound_SHIPS.Length)];
                    break;
            }

            ambienceAS.Play();
        }
        private void StopAmbience()
        {
            ambienceAS.Stop();
        }
        #endregion
    }

    public enum AmbientType
    {
        OnSea = 0,
        OnTavern = 1,
        OnCity = 2
    }

    public interface IContainsMusicList
    {
        public AudioClip[] GetSoundtrack();
    }
}

