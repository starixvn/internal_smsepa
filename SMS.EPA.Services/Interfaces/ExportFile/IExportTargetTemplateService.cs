namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Model;

    public interface IExportTargetTemplateService
    {
        void Export(TargetTemplate targetTemplate, IEnumerable<TargetTemplateKPIGroupModel> kpis, string templatePath);
    }
}
