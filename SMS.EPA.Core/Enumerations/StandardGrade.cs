namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum StandardGrade
    {
        None = 0,

        [Description("A")]
        Excellent = 1,

        [Description("B")]
        Good = 2,

        [Description("C")]
        Average = 3,

        [Description("D")]
        BellowAverage = 4,

        [Description("E")]
        Poor = 5
    }
}