namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum EpaStatus
    {
        [Description("Open")]
        Open = 0,

        [Description("Closed")]
        Closed = 1,

        [Description("Rejected")]
        Rejected = 2
    }
}