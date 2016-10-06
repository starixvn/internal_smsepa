namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class GetPeriodHasReminderProcessCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GetPeriodHasReminderProcess";
        }
    }
}