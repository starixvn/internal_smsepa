using System.ComponentModel;

namespace SMS.EPA.Core.Enumerations
{
    public enum JobLevelAssessmentStatus
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Verified")]
        Verified,

        [Description("Reviewed")]
        Approved
    }
}