using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using GameSettings.Core;

namespace GameMechanics.Sound
{
    public class Music : AmbienceBehaviour<Music>, IContainsMusicList
    {
        [Header("As Menu Tracklist Controller")]
        //Data
        [SerializeField] private Song[] _tracks;

        //Event
        public delegate void ChangeSong(Song song);
        public static ChangeSong NewSong;
        protected override void Start()
        {
            //Modding
            if (PersistentGameSettings.currentMod != null)
            {
                var delivery = GameObject.FindWithTag("Delivery").GetComponent<MenuMusicDelivery>();

                _tracks = delivery.Deliver();
                _songNames = new string[_tracks.Length];
                for (int i = 0; i < _tracks.Length; i++) { _songNames[i] = _tracks[i].songName; }
            }

            base.Start();

            //Start Music playlist
            StartPlayList(_tracks.Length);

            TrackRowScript.SelectTrack += PlaySelectedTrack;
            TrackRowScript.SkipCurrent += SkipTrack;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TrackRowScript.SelectTrack -= PlaySelectedTrack;
            TrackRowScript.SkipCurrent -= SkipTrack;
        }

        protected override void NextTrack()
        {
            if (TrackRowScript.activeTracks > 0)
            {
                currentTrack = (byte)(currentTrack == (_tracks.Length - 1) ? 0 : currentTrack + 1);
            }
            else
            {
                StopCoroutine("MusicControl");
            }
        }

        public override AudioClip[] GetSoundtrack()
        {
            var music = new AudioClip[_tracks.Length];
            for (int i = 0; i < _tracks.Length; i++)
            {
                music[i] = _tracks[i].track;
            }
            return music;
        }

        protected override AudioClip SetCurrentClip()
        {
            return _tracks[currentTrack].track;
        }

        protected override void GetClip()
        {
            base.GetClip();

            if (TrackRowScript.activeTracks > 0)
            {
                if (TrackRowScript.isActiveTrack(layoutContainer, currentTrack))
                {
                    NewSong(_tracks[currentTrack]);
                }
            }
        }
    }

    [System.Serializable]
    public class Song
    {
        public AudioClip track;
        public string songName;
        public string description;

        public Song(AudioClip track, string songName, string description)
        {
            this.track = track;
            this.songName = songName;
            this.description = description;
        }

        public Song() { }
    }
}
