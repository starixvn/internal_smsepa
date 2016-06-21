namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum StandardGrade : int
    {
        None = 0,

        [Description("A")]
        Excellent = 1,

        [Description("B")]
        Good = 2,

        [Description("C")]
        Standard = 3,

        [Description("D")]
        UnderPerformance = 4,

        [Description("E")]
        Fail = 5
    }
}