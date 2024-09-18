using UnityEngine;
using UnityEngine.Rendering;


namespace swatchr
{
    public class SwatchrAmbientLightColor : SwatchrColorApplier
    {
        protected override void Apply()
        {
            var triLightingColor = FindObjectOfType<SwatchrAmbientTriLightingColor>();
            if (triLightingColor != null && triLightingColor.enabled)
            {
                Debug.LogWarning("[SwatchrAmbientLightColor] SwatchrAmbientTriLightingColor is present in the scene. These two cannot co-exist in the same scene. Disabling SwatchrAmbientTriLightingColor.");
                triLightingColor.gameObject.SetActive(false);
            }
            
            if (RenderSettings.ambientMode != AmbientMode.Flat)
            {
                RenderSettings.ambientMode = AmbientMode.Flat;
            }

            RenderSettings.ambientLight = swatchrColor.color;
        }
    }
}