namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

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