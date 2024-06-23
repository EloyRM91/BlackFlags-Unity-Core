using UnityEngine;

namespace GameMechanics.Sound
{
    public class Music2 : MonoBehaviour
    {
        public Song menuTheme;
        private AudioSource _thisAS;

        //Event
        public delegate void ChangeSong(Song song);
        public static ChangeSong NewSong;
        void Start()
        {
            _thisAS = GetComponent<AudioSource>();

            Invoke("Delay", 0.05f);
        }
        private void Delay() { NewSong(menuTheme); _thisAS.Play(); }

    }
}
