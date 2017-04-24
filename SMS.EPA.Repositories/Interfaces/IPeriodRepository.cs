namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public interface IPeriodRepository
    {
        IEnumerable<PeriodModel> GetPeriod(ICriteria criteria);

        PeriodModel GetPeriodById(ICriteria criteria);

        IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(ICriteria criteria);

        IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(ICriteria criteria);
    }
}