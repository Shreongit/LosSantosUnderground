namespace LosSantosUnderground.Core
{
    public enum RaceAbortReason
    {
        None = 0,
        PoliceDetected = 1,
        PlayerLeft = 2,
        InvalidRace = 3,
        TimeWindowClosed = 4,
        PlayerDied = 5,
        VehicleDestroyed = 6,
        Cancelled = 7
    }
}