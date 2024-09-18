using UnityEngine;


namespace swatchr
{
    [RequireComponent(typeof(ParticleSystem))]
    public class SwatchrParticleSystem : SwatchrColorApplier
    {
        private ParticleSystem swatchingParticleSystem;


        protected override void Apply()
        {
            if (swatchingParticleSystem == null)
            {
                swatchingParticleSystem = GetComponent<ParticleSystem>();
            }

            var main = swatchingParticleSystem.main;
            main.startColor = swatchrColor.color;
        }
    }
}