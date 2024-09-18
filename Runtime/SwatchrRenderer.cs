using UnityEngine;


namespace swatchr
{
    /// <summary>
    ///     Only use in Built-In Render Pipeline. Use URP version for URP.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class SwatchrRenderer : SwatchrColorApplier
    {
        [HideInInspector]
        public Renderer swatchingRenderer;

        private static MaterialPropertyBlock mpb;
        private static int colorShaderId;


        protected override void Apply()
        {
            if (mpb == null)
            {
                mpb = new MaterialPropertyBlock();
                colorShaderId = Shader.PropertyToID("_Color");
            }

            if (swatchingRenderer == null)
            {
                swatchingRenderer = GetComponent<Renderer>();
            }

            mpb.SetColor(colorShaderId, swatchrColor.color);

            swatchingRenderer.SetPropertyBlock(mpb);
        }
    }
}