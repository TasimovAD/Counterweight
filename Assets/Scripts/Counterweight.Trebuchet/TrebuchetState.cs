namespace Counterweight.Trebuchet
{
    /// <summary>
    /// High-level lifecycle of a single shot. Future iterations may add
    /// explicit WindingUp/Armed/Resetting transitions; for the MVP only
    /// Idle -> Firing -> Released -> Idle is exercised.
    /// </summary>
    public enum TrebuchetState
    {
        Idle,
        WindingUp,
        Armed,
        Firing,
        Released,
        Resetting
    }
}
