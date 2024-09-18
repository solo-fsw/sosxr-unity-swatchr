using System;
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
        [HideInInspector] public int skyColorIndex = 0;
        [HideInInspector] public int equatorColorIndex = 1;
        [HideInInspector] public int groundColorIndex = 2;

        [HideInInspector] public Color[] swatchColors = Array.Empty<Color>();


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

            Debug.Log("[SwatchrAmbientTriLightingColor] Color index out of range");

            return Color.black; // Default to black if out of range
        }
    }
}