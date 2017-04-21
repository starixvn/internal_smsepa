namespace SMS.EPA.Services.Interfaces
{
    using Model;

    public interface IDashboardService
    {
        ToDoListViewModel GetToDoList(int employeeId);
    }
}