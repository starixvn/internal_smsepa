namespace SMS.EPA.Core.Repositories
{
    using Fanex.Data.Repository;
    using Model;

    public interface IBaseRepository
    {
        CommandResult Insert(ICriteria criteria);

        CommandResult Update(ICriteria criteria);

        CommandResult Delete(ICriteria criteria);
    }
}