namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum ProcessType
    {
        None = 0,

        [Description("Human Resource")]
        HumanResource,

        [Description("Self")]
        Self,

        [Description("Evaluator")]
        Evaluator,

        [Description("Reviewer")]
        Reviewer,

        [Description("Management")]
        Management
    }
}