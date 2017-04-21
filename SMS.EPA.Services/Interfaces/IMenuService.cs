namespace SMS.EPA.Services.Interfaces
{
    using Model;
    using System.Collections.Generic;

    public interface IMenuService
    {
        IList<MenuGroupModel> GetMenusByEmployeeId(int employeeId);
    }
}