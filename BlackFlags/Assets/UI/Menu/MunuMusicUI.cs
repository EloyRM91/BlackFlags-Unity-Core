using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Sound
{

    public class MunuMusicUI : MonoBehaviour
    {
        [SerializeField] private Image menuBackground;

        private Text _Text_song, _Text_description;
        private Transform _menuPanel;
        private void Awake()
        {
            Music.NewSong += SetSongInfo;
            Music2.NewSong += SetSongInfo;
        }
        void Start()
        {
            _menuPanel = transform.GetChild(0);
            _Text_song = _menuPanel.GetChild(1).GetComponent<Text>();
            _Text_description = _menuPanel.GetChild(2).GetComponent<Text>();
        }

        private void SetSongInfo(Song song)
        {
            _Text_song.text = song.songName;
            _Text_description.text = song.description;
            _menuPanel.gameObject.SetActive(true);
            GetComponent<Animator>().SetTrigger("MovePanel");
        }

        public void SetOffPanel()
        {
            _menuPanel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Music.NewSong -= SetSongInfo;
            Music2.NewSong -= SetSongInfo;
        }
    }
}

