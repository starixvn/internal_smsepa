namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum Policy
    {
        [Description("None")]
        None = 0,

        [Description("Read")]
        Read = 1,

        [Description("Host")]
        Host = 2
    }
}