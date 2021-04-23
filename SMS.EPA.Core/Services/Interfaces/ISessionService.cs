namespace SMS.EPA.Core.Services
{
    using Enumerations;
    using Model;

    public interface ISessionService
    {
        int EmployeeId { get; set; }

        UserSession CurrentUser { get; set; }

        byte GetPermission(string functionKey);

        bool HasPermissionOnFunction(string functionKey, Policy policy);
    }
}