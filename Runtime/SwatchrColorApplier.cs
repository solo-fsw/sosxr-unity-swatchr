using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace swatchr
{
    [ExecuteInEditMode]
    public abstract class SwatchrColorApplier : MonoBehaviour
    {
        public SwatchrColor swatchrColor;

        private const string DefaultSwatchAssetName = "Project Wide Swatch"; // Update this to match your asset name


        private void OnEnable()
        {
            if (swatchrColor == null)
            {
                // AssignDefaultSwatch();
            }

            if (swatchrColor != null)
            {
                swatchrColor.OnColorChanged += Apply;
                swatchrColor.OnEnable();
            }
        }


        protected virtual void OnDisable()
        {
            if (swatchrColor != null)
            {
                swatchrColor.OnColorChanged -= Apply;
            }
        }


        private void OnDestroy()
        {
            if (swatchrColor != null)
            {
                swatchrColor.OnColorChanged -= Apply;
            }
        }


        #if UNITY_EDITOR
        private void AssignDefaultSwatch()
        {
            if (swatchrColor != null && swatchrColor.swatch != null)
            {
                return;
            }

            // Find the asset by name and filter by Swatch type
            var guids = AssetDatabase.FindAssets($"t:Swatch {DefaultSwatchAssetName}");

            if (guids.Length <= 0)
            {
                Debug.LogWarning($"Default Swatch asset '{DefaultSwatchAssetName}' not found.");

                return;
            }

            // Load the first matching Swatch asset
            var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            var defaultSwatch = AssetDatabase.LoadAssetAtPath<Swatch>(assetPath);

            if (defaultSwatch != null)
            {
                swatchrColor ??= new SwatchrColor();

                swatchrColor.swatch = defaultSwatch;

                Debug.Log($"Assigned default Swatch: {defaultSwatch.name}");
            }
            else
            {
                Debug.LogWarning($"Failed to load Swatch asset at path '{assetPath}'.");
            }
        }
        #endif


        protected abstract void Apply();


        #if UNITY_EDITOR
        private void OnValidate()
        {
            AssignDefaultSwatch();
        }


        private void Reset()
        {
            AssignDefaultSwatch();
        }
        #endif
    }
}