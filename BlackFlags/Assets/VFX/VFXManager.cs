using UnityEngine;


namespace GameSettings.VFX
{
    public class VFXManager : MonoBehaviour
    {
        void Awake()
        {
            GraphicSettings.postProcessingChange += AllowPostProcessing;
        }

        void OnDestroy()
        {
            GraphicSettings.postProcessingChange -= AllowPostProcessing;
        }

        private void AllowPostProcessing()
        {
            var value = GraphicSettings.Postprocessing;
            foreach (Transform effect in transform)
            {
                effect.gameObject.SetActive(value);
            }
        }
    }
}

