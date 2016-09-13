namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IMenuService
    {
        IList<MenuGroupModel> GetMenusByEmployeeId(int employeeId);
    }
}