using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

namespace GameMechanics.Intro
{
    public class LogoTransition : Transition
    {
        [SerializeField] private Image panel;
        private void Start()
        {
            panel.DOFade(1, 1f);
            player = player.transform.GetComponent<AudioSource>();
        }

        private void FixedUpdate()
        {
            if (!((AudioSource)player).isPlaying)
            {
                Load();
            }
        }

    }
}

