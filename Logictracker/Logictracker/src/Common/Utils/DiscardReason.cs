namespace Logictracker.Utils
{
    #region Public Enums

    public enum DiscardReason
    {
        None = 0,
        NoAssignedMobile = 1,
        InvalidDate = 2,
        OutOfGlobe = 3,
        InvalidSpeed = 4,
        InvalidDistance = 5,
        LowQualitySignal = 6,
        Exception = 7,
        DatamartCleanUp = 8,
        Manual = 9,
        NoAssignedDevice = 10,
        NoMessageFound = 11,
        InsideInhibitor = 12,
        MissingPositions = 13,
		InvalidDuration = 14,
		InvalidTicketData = 15,
        SeguimientoNoAutorizado = 16,
        LowerQualitySignalOnSpeed0 = 17,
        DuplicatedPosition = 18
    }

    #endregion
}
