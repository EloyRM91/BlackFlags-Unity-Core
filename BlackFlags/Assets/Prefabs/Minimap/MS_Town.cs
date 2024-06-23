//Core
using UnityEngine;
//Get time speed from Time Manager
using GameMechanics.Data;
//Tweening
using DG.Tweening;

namespace GameMechanics.WorldCities
{
    public class MS_Town : MinimapSprite
    {
        private static Color spriteColor;
        private void Start()
        {
            spriteColor = new Color(1, 0.196f, 0.9f, 1);
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

