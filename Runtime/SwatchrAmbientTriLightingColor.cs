using UnityEngine;
using UnityEngine.Rendering;


namespace swatchr
{
    [ExecuteInEditMode]
    public class SwatchrAmbientTriLightingColor : MonoBehaviour
    {
        [Header("Warning: This component changes scene settings in Lighting->Scene")]
        public SwatchrColor sky;
        public SwatchrColor equator;
        public SwatchrColor ground;


        private void OnDestroy()
        {
            sky.OnColorChanged -= Apply;
            equator.OnColorChanged -= Apply;
            ground.OnColorChanged -= Apply;
        }


        private void OnDisable()
        {
            sky.OnColorChanged -= Apply;
            equator.OnColorChanged -= Apply;
            ground.OnColorChanged -= Apply;
        }


        private void OnEnable()
        {
            sky ??= new SwatchrColor();

            equator ??= new SwatchrColor();

            ground ??= new SwatchrColor();

            sky.OnColorChanged += Apply;
            sky.OnEnable();

            equator.OnColorChanged += Apply;
            equator.OnEnable();

            ground.OnColorChanged += Apply;
            ground.OnEnable();
        }


        public void Apply()
        {
            if (RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                Debug.LogWarning("[SwatchrAmbientTryLightingColor] RenderSettings.ambientMode != Trilight. Changing the setting to Tri Lighting. Change it manually in Lighting->Scene.");
                RenderSettings.ambientMode = AmbientMode.Trilight;
            }

            RenderSettings.ambientSkyColor = sky.color;
            RenderSettings.ambientEquatorColor = equator.color;
            RenderSettings.ambientGroundColor = ground.color;
        }
    }
}