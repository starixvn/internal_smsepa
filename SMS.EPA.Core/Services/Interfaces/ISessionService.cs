namespace SMS.EPA.Core.Services
{
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;

    public interface ISessionService
    {
        int EmployeeId { get; set; }

        UserSession CurrentUser { get; set; }

        int GetPermission(string functionKey);

        bool HasPermissionOnFunction(string functionKey, Policy policy);
    }
}