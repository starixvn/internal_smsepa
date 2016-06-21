namespace SMS.EPA.Web.Services
{
    using SMS.EPA.Core.Enumerations;

    public interface IWebCommonService
    {
        int GetPermission(string functionKey);

        bool HasPermissionOnFunction(string functionKey, Policy policy);
    }
}