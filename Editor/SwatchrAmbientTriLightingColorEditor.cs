using UnityEditor;
using UnityEngine;


namespace swatchr
{
    /// <summary>
    ///     SOSXR
    /// </summary>
    [CustomEditor(typeof(SwatchrAmbientTriLightingColor))]
    public class SwatchrAmbientTriLightingColorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (SwatchrAmbientTriLightingColor) target;

            if (script.swatchrColor != null)
            {
                EditorGUILayout.ObjectField("Swatch", script.swatchrColor.swatch, typeof(Swatch), false);
            }
            else
            {
                EditorGUILayout.LabelField("No SwatchrColor assigned", EditorStyles.helpBox);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Sky", EditorStyles.boldLabel);
            script.skyColorIndex = EditorGUILayout.IntSlider("Sky Color Index", script.skyColorIndex, 0, Mathf.Max(0, script.swatchColors.Length - 1));
            EditorGUILayout.ColorField("Color Sky", script.swatchColors[script.skyColorIndex]);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Equator", EditorStyles.boldLabel);
            script.equatorColorIndex = EditorGUILayout.IntSlider("Equator Color Index", script.equatorColorIndex, 0, Mathf.Max(0, script.swatchColors.Length - 1));
            EditorGUILayout.ColorField("Color Equator", script.swatchColors[script.equatorColorIndex]);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Ground", EditorStyles.boldLabel);
            script.groundColorIndex = EditorGUILayout.IntSlider("Ground Color Index", script.groundColorIndex, 0, Mathf.Max(0, script.swatchColors.Length - 1));
            EditorGUILayout.ColorField("Color Ground", script.swatchColors[script.groundColorIndex]);

            serializedObject.ApplyModifiedProperties();
        }
    }
}