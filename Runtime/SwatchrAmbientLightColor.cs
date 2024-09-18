using UnityEngine;
using UnityEngine.Rendering;


namespace swatchr
{
    public class SwatchrAmbientLightColor : SwatchrColorApplier
    {
        public override void Apply()
        {
            if (RenderSettings.ambientMode != AmbientMode.Flat)
            {
                Debug.LogWarning("[SwatchrAmbientLightColor] Ambient light mode is not set to flat. Changing it now. Change it manually in Lighting->Scene.");
                RenderSettings.ambientMode = AmbientMode.Flat;
            }

            RenderSettings.ambientLight = swatchrColor.color;
        }
    }
}