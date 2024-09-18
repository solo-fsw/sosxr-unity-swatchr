using UnityEngine;


namespace swatchr
{
    /// <summary>
    ///     SOSXR
    ///     Only use in URP. Use default version for Built-In Render Pipeline.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class SwatchrRendererURP : SwatchrColorApplier
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
                colorShaderId = Shader.PropertyToID("_BaseColor");
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