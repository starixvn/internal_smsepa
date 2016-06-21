namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum ProcessType : int
    {
        None = 0,

        [Description("Human Resource")]
        HumanResource = 1,

        [Description("Self")]
        Self = 2,

        [Description("Evaluator")]
        Evaluator = 3,

        [Description("Reviewer")]
        Reviewer = 4,

        [Description("Management")]
        Management = 5
    }
}