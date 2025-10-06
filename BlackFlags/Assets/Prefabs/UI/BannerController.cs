using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldMap
{
    public class BannerController : MonoBehaviour
    {
        [SerializeField] protected Transform _target;
        public static Camera cam;

        public Transform Target
        {
            get
            {
                return _target;
            }
        }

        protected virtual void Update()
        {
            SetUIPosition();
        }

        protected virtual void SetUIPosition()
        {
            transform.position = cam.WorldToScreenPoint(_target.position);
        }

        public bool HasTarget()
        {
            return _target != null;
        }

        public void ForcePosition()
        {
            SetUIPosition();
        }

        public void SetNewTarget(Transform newTarget)
        {
            _target = newTarget;
        }

        public void SetNewText(string value)
        {
            var txt = GetText();
            if (txt)
            {
                txt.text = value;
            }
        }

        public void SetImage(Sprite newImg)
        {
            var renderer = getFlagImage();
            renderer.sprite = newImg;
        }

        private Text GetText()
        {
            Text Traverse(Transform tr)
            {
                Text textComponent = tr.GetComponent<Text>();
                if (textComponent != null)
                    return textComponent;

                foreach (Transform child in tr)
                {
                    Text found = Traverse(child);
                    if (found != null)
                        return found;
                }

                return null;
            }

            return Traverse(transform);
        }

        private Image getFlagImage()
        {
            Image Traverse(Transform tr)
            {
                Image img = tr.GetComponent<Image>();
                if (img != null)
                    if (tr.name == "Image - Kingdom")
                        return img;

                foreach (Transform child in tr)
                {
                    Image found = Traverse(child);
                    if (found != null)
                        return found;
                }

                return null;
            }

            return Traverse(transform);
        }
    }
}
