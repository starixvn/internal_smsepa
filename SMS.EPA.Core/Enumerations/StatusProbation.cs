namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum StatusProbation
    {
        [Description("Fail Probation")]
        FailProbation = 0,

        [Description("Pass Probation")]
        PassProbation = 1,

        [Description("Extend Probation")]
        ExtendProbation = 2
    }
}