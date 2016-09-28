namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Model;

    public interface IDashboardService
    {
        ToDoListViewModel GetToDoList(int employeeId);
    }
}