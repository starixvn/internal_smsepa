namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.KPI;
    using SMS.EPA.Model.KPI;

    public interface IKPIService
    {
        IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria);

        CommandResult CreateKPI(CreateKPICriteria criteria);

        CommandResult UpdateKPI(UpdateKPICriteria criteria);

        CommandResult UpdateStatusKPI(UpdateStatusKPICriteria criteria);

        CommandResult UpdateRequiredEvaluateKPI(UpdateRequiredEvaluateKPICriteria criteria);

        CommandResult DeleteKPI(DeleteKPICriteria criteria);
    }
}