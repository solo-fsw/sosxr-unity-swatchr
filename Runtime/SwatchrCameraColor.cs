using UnityEngine;


namespace swatchr
{
    [RequireComponent(typeof(Camera))]
    public class SwatchrCameraColor : SwatchrColorApplier
    {
        [HideInInspector]
        public Camera swatchingCamera;


        protected override void Apply()
        {
            if (swatchingCamera == null)
            {
                swatchingCamera = GetComponent<Camera>();
            }

            swatchingCamera.clearFlags = CameraClearFlags.SolidColor;
            swatchingCamera.backgroundColor = swatchrColor.color;
        }
    }
}