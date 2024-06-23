using UnityEngine;
using DG.Tweening;
using GameMechanics.Data;

namespace GameMechanics.WorldCities
{
    public class MS_NaturalPort : MinimapSprite
    {
        private static Color spriteColor;
        private void Start()
        {
            spriteColor = new Color(0.12f, 0.65f, 0, 1);
        }

        public override void OnMouseExit()
        {
            if (!isSelected)
                GetComponent<SpriteRenderer>().color = spriteColor;
        }

        public override void Select()
        {
            base.Select();
            var sprite = GetComponent<SpriteRenderer>();
            //Loop
            sequence = DOTween.Sequence();
            sequence.Append(sprite.DOColor(Color.white, 0.8f)).SetUpdate(true);
            sequence.Append(sprite.DOColor(spriteColor, 0.8f)).SetUpdate(true);

            sequence.SetLoops(-1);
            sequence.Play();
        }

        public override void Unselect()
        {
            base.Unselect();
            GetComponent<SpriteRenderer>().color = spriteColor;
        }
    }
}

