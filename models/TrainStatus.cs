namespace IpisCentralDisplayController.models
{
    public enum TrainStatus
    {
        // Arrival statuses
        RunningRightTimeArrival,
        WillArriveShortly,
        IsArrivingOn,
        HasArrivedOn,
        RunningLateArrival,
        CancelledArrival,
        IndefiniteLateArrival,
        TerminatedAt,
        PlatformChangedArrival,

        // Departure statuses
        RunningRightTimeDeparture,
        CancelledDeparture,
        IsReadyToLeave,
        IsOnPlatform,
        Departed,
        Rescheduled,
        Diverted,
        DelayDeparture,
        PlatformChangeDeparture,
        ChangeOfSource
    }
}
