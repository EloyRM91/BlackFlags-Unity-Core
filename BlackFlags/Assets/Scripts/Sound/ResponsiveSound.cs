using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Sound
{
    [RequireComponent(typeof(Button))]
    public abstract class ResponsiveSound : MonoBehaviour
    {
        public static bool adapt2TEA;

        public delegate void ResponseEvent(AudioResponse type);
        public static event ResponseEvent AudioResponse;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => Response());
        }

        protected abstract void Response();

        public static void CallResponse(AudioResponse typeOfResponse)
        {
            if(!adapt2TEA)
                AudioResponse(typeOfResponse);
        }
    }

    public enum AudioResponse { click1, click2, coin1, coin2}
}

