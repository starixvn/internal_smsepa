namespace SMS.EPA.Services.Interfaces
{
    public interface IExportPersonalFlowSettingService
    {
        void Export(int flowTemplateId, string flowTemplateName);
    }
}