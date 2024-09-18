using UnityEngine;
using Zaikman;


namespace swatchr
{
    /// <summary>
    ///     SOSXR
    ///     URP version of SwatchrMultiRenderer. Use default version for Built-In Render Pipeline.
    /// </summary>
    public class SwatchrMultiRendererURP : SwatchrColorApplier
    {
        public Renderer[] renderers;

        // you need a fake public field for this attribute to work
        // string is method name, field name is button text
        [InspectorButton("GatherChildren")]
        public bool GatherChildRenderers;

        private static MaterialPropertyBlock mpb;
        private static int colorShaderId;


        public void GatherChildren()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }


        protected override void Apply()
        {
            if (mpb == null)
            {
                mpb = new MaterialPropertyBlock();

                colorShaderId = Shader.PropertyToID("_BaseColor");
            }

            mpb.SetColor(colorShaderId, swatchrColor.color);

            if (renderers != null)
            {
                for (var i = 0; i < renderers.Length; i++)
                {
                    renderers[i].SetPropertyBlock(mpb);
                }
            }
        }
    }
}