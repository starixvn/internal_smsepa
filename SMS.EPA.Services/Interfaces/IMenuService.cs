namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IMenuService
    {
        IEnumerable<MenuGroupModel> GetMenusByEmployeeId(int employeeId);
    }
}