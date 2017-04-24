namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public interface IDashboardRepository
    {
        IEnumerable<ToDoListModel> GetToDoList(ICriteria criteria);
    }
}