using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

namespace GameMechanics.Intro
{
    public class PegiTransition : MonoBehaviour
    {
        [SerializeField] private Image panel;
        private Sequence sequence;
        void Start()
        {
            sequence = DOTween.Sequence();
            sequence.Append(panel.DOFade(0, 1f));
            sequence.AppendInterval(3);
            sequence.Append(panel.DOFade(1, 1f));
            sequence.AppendCallback(delegate { SceneManager.LoadScene(2); });
        }

    }
}
