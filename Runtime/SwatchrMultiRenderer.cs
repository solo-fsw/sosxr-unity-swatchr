using UnityEngine;
using Zaikman;


namespace swatchr
{
    public class SwatchrMultiRenderer : SwatchrColorApplier
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


        public override void Apply()
        {
            if (mpb == null)
            {
                mpb = new MaterialPropertyBlock();
                colorShaderId = Shader.PropertyToID("_Color");
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