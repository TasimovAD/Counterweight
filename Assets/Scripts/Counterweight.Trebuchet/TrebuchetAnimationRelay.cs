using System;
using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Receives Animation Events from the trebuchet's Animator and re-broadcasts them
    /// as plain C# events. Decouples <see cref="TrebuchetController"/> from the animation
    /// system and lets multiple listeners (controller, audio, VFX) subscribe later.
    ///
    /// Required animation events:
    ///   - <c>OnWindUpComplete</c>  on the last frame of the WindUp clip.
    ///   - <c>OnProjectileRelease</c> on the frame where the sling lets go in the Fire clip.
    /// </summary>
    public sealed class TrebuchetAnimationRelay : MonoBehaviour
    {
        public event Action WindUpComplete;
        public event Action ProjectileRelease;

        // Called by Unity Animation Event. Names must match exactly.
        public void OnWindUpComplete()
        {
            WindUpComplete?.Invoke();
        }

        public void OnProjectileRelease()
        {
            ProjectileRelease?.Invoke();
        }
    }
}
