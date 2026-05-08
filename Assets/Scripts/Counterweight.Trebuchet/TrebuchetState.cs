namespace Counterweight.Trebuchet
{
    /// <summary>
    /// Lifecycle of a single shot.
    /// Player advances through the active states by interacting with parts of
    /// the trebuchet (winch, ammo basket, release lever).
    /// </summary>
    public enum TrebuchetState
    {
        Idle,        // counterweight down, arm up — player can crank the winch
        WindingUp,   // wind-up animation playing
        Armed,       // counterweight up, arm down — player can place a stone
        Loaded,      // stone in basket — player can pull the release lever
        Firing,      // fire animation playing, projectile spawns mid-clip
        Released,    // post-fire settle
        Resetting    // returning to idle (currently a delay-based timer)
    }
}
