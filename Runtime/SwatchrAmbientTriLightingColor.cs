using System;
using Unity.VisualScripting.YamlDotNet.Serialization.ObjectGraphVisitors;
using UnityEngine;
using UnityEngine.Rendering;


namespace swatchr
{
    /// <summary>
    ///     SOSXR
    /// </summary>
    [ExecuteInEditMode]
    public class SwatchrAmbientTriLightingColor : SwatchrColorApplier
    {
        public int skyColorIndex = 0;
        public int equatorColorIndex = 1;
        public int groundColorIndex = 2;

        [HideInInspector]
        public Color[] swatchColors = Array.Empty<Color>();


        private void OnDestroy()
        {
            if (swatchrColor != null)
            {
                swatchrColor.OnColorChanged -= Apply;
            }
        }


        private void OnDisable()
        {
            if (swatchrColor != null)
            {
                swatchrColor.OnColorChanged -= Apply;
            }
        }


        private void OnEnable()
        {
            swatchrColor ??= new SwatchrColor();

            if (swatchrColor != null)
            {
                swatchrColor.OnColorChanged += UpdateSwatchColors;
                swatchrColor.OnEnable();
                UpdateSwatchColors();
            }
        }


        private void UpdateSwatchColors()
        {
            // Assuming SwatchrColor has a property or method to get the colors
            swatchColors = swatchrColor.swatch.colors;
            Apply();
        }


        protected override void Apply()
        {
            var singleAmbientLightColor = FindObjectOfType<SwatchrAmbientLightColor>();
            if (singleAmbientLightColor != null && singleAmbientLightColor.enabled)
            {
                Debug.LogWarning("[SwatchrAmbientTriLightingColor] SwatchrAmbientLightColor is present in the scene. These two cannot co-exist in the same scene. Disabling the other one");
                singleAmbientLightColor.gameObject.SetActive(false);
            }
            
            if (RenderSettings.ambientMode != AmbientMode.Trilight)
            {
                RenderSettings.ambientMode = AmbientMode.Trilight;
            }

            RenderSettings.ambientSkyColor = GetColor(skyColorIndex);
            RenderSettings.ambientEquatorColor = GetColor(equatorColorIndex);
            RenderSettings.ambientGroundColor = GetColor(groundColorIndex);
        }


        private Color GetColor(int index)
        {
            if (swatchColors != null && index >= 0 && index < swatchColors.Length)
            {
                return swatchColors[index];
            }

            return Color.black; // Default to black if out of range
        }
    }
}