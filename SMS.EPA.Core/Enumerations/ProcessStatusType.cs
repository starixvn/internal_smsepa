namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum ProcessStatusType
    {
        [Description("None")]
        None = 0,

        [Description("Open")]
        Open = 1,

        [Description("In Progress")]
        InProgress = 2,

        [Description("Closed")]
        Closed = 3
    }
}