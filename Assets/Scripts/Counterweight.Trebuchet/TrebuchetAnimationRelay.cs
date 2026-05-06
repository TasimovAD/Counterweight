using System;
using UnityEngine;

namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Receives Animation Events from the trebuchet's Animator and re-broadcasts them
    /// as plain C# events. Decouples <see cref="TrebuchetController"/> from the animation
    /// system and lets multiple listeners (controller, audio, VFX) subscribe later.
    ///
    /// The animator's Fire clip must contain an Animation Event named
    /// <c>OnProjectileRelease</c> at the frame where the sling lets go.
    /// </summary>
    public sealed class TrebuchetAnimationRelay : MonoBehaviour
    {
        public event Action ProjectileRelease;

        // Called by Unity Animation Event. Name must match exactly.
        public void OnProjectileRelease()
        {
            ProjectileRelease?.Invoke();
        }
    }
}
