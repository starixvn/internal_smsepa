namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface IDashboardRepository
    {
        IEnumerable<ToDoListModel> GetToDoList(ICriteria criteria);
    }
}