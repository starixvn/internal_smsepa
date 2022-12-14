namespace SMS.EPA.AutoJob.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using SMS.EPA.AutoJob.Models;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command);
    }
}