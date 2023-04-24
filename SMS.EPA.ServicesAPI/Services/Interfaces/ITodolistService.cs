namespace SMS.EPA.ServicesAPI.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SMS.EPA.ServicesAPI.Models.Todolist;

    public interface ITodolistService
    {
        Task<IList<TodolistItem>> GetEPAByUserIdAsync(int userId);
    }
}