namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Model;

    public interface IExportPersonalFlowSettingService
    {
        void Export(IEnumerable<PersonalFlowModel> flowSettings, string flowTemplateName);
    }
}