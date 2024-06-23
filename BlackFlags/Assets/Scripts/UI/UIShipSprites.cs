using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//Mechanics
using GameMechanics.AI;

namespace UI.WorldMap
{
    /// <summary>
    /// An ship banner controller class
    /// Parameters: sprites (Sprite[])
    ///   * Constructors: None (Monobehaviour)
    /// </summary>
    public class UIShipSprites : BannerController
    {
        [Header("Boarding Sprites")]
        [SerializeField] private Sprite[] _sprites;

        [Header("Components")]
        [SerializeField] private Image _IMG_Frame;
        [SerializeField] private Image _IMG_flag;
        [SerializeField] private Text _TEXT_convoySize;

        //Frame colors
        private static readonly Color
            color_Pirate = new Color(0.22f, 0.22f, 0.22f, 1),
            color_GreatMerchant = new Color(0.422f, 0, 0.393f, 1),
            color_Military = new Color(0.877f, 0.272f, 0.254f, 1);
        protected void OnEnable()
        {
            StartCoroutine("OnEnableSync");
        }
        protected void OnDisable()
        {
            transform.position = Vector3.back * 4000; // 09/12/2022
        }
        IEnumerator OnEnableSync()
        {
            while(_target == null)
            {
                yield return null;
            }
            SetSprite();
        }

        public void SetAsTarget(Transform newTarget)
        {
            _target = newTarget;
        }
        public void Release()
        {
            _target = null;
        }
        public void SetSprite()
        {
            int index = 0;
            var rot = _target.rotation.eulerAngles.y;

            if (rot < 11 || rot >= 348.75f)
            {
                index = 0;
            }
            else
            {
                for (int i = 1; i < 15; i++)
                {
                    var limSup = 22.5f * i + 11.25f;
                    var limInf = 22.5f * i - 11.25f;
                    if (rot < limSup && rot >= limInf)
                    {
                        index = 16 - i;
                        break;
                    }
                }
            }
            GetComponent<Image>().sprite = _sprites[index];
        }
        public void SetValues(Sprite spt, byte n, ClassAI IA)
        {
            _IMG_flag.sprite = spt;
            _TEXT_convoySize.text = n.ToString();

            if(IA is AI_Pirate)
            {
                _IMG_Frame.color = color_Pirate;
            }
            else if(IA is AI_LocalMerchant)
            {
                _IMG_Frame.color = Color.white;
            }
            else if(IA is AI_Merchant)
            {
                _IMG_Frame.color = Color.green;
            }
            else
            {
                _IMG_Frame.color = color_Military;
            }
        }

        public void SetSpritesSet(byte n)
        {
            for (int i = 0; i < _sprites.Length; i++)
                _sprites[i] = Resources.Load<Sprite>($"Boarding/{n}/{n}_{i}");
        }
    }
}

