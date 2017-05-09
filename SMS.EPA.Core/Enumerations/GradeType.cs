namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum GradeType
    {
        None = 0,

        [Description("Discipline")]
        Discipline = 1,

        [Description("Performance")]
        Performance = 2,

        [Description("Final")]
        Final = 3
    }
}