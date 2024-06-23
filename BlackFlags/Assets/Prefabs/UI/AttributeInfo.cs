using UnityEngine.UI;
using GameMechanics.Data;

namespace UI.WorldMap
{
    public class AttributeInfo : ButtonInfoOnTarget
    {
        public CharacterAttribute attribute;
        void Start()
        {
            
        }
        public override void DisplayInfo()
        {
            _TEXT_targetText.text = $"{attribute.name} (atributo) - {attribute.description}";
        }
        public void SetTextTarget(Text target)
        {
            _TEXT_targetText = target;
        }
    }
}

