namespace SMS.EPA.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Model;

    public interface IBaseService
    {
        IEnumerable<T> GetData<T>(ICriteria criteria);

        Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria);

        T GetDataEntity<T>(ICriteria criteria);

        Task<T> GetDataEntityAsync<T>(ICriteria criteria);

        Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>> FetchMultiple<TFirst, TSecond>(ICriteria criteria);

        Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria);

        Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>> FetchMultiple<TFirst, TSecond, TThird>(ICriteria criteria);

        Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria);

        CommandResult ExecuteNonQuery(ICriteria criteria);

        Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria);
    }
}