namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

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