using UnityEngine;


namespace swatchr
{
    [ExecuteInEditMode]
    public abstract class SwatchrColorApplier : MonoBehaviour
    {
        public SwatchrColor swatchrColor;


        private void OnDestroy()
        {
            swatchrColor.OnColorChanged -= Apply;
        }


        private void OnDisable()
        {
            swatchrColor.OnColorChanged -= Apply;
        }


        private void OnEnable()
        {
            swatchrColor ??= new SwatchrColor();

            swatchrColor.OnColorChanged += Apply;
            swatchrColor.OnEnable();
        }


        public abstract void Apply();
    }
}