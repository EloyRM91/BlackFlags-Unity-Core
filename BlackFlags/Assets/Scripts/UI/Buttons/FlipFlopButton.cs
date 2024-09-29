using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.UI
{
    //[RequireComponent(typeof(Button))]
    //public class FlipFlopButton<T> : GameButton where T: Button
    public class FlipFlopButton : GameButton
    {
        [SerializeField] private GameObject _target;

        protected override void Start()
        {
            var button = GetComponent<Button>();

            if(button != null)
            {
                button.onClick.AddListener(() => _target.SetActive(!_target.activeSelf));
            }
        }
    }
}
