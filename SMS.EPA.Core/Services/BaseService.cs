namespace SMS.EPA.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Repositories;

    public class BaseService
    {
        private readonly IDynamicRepository dynamicRepository;

        public BaseService(IDynamicRepository dynamicRepository = null, IBaseRepository repository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            Repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository { get; set; }

        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Fetch<T>(criteria);
            }

            return new List<T>();
        }

        public virtual async Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchAsync<T>(criteria);
            }

            return new List<T>();
        }

        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Get<T>(criteria);
            }

            return default(T);
        }

        public virtual async Task<T> GetDataEntityAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.GetAsync<T>(criteria);
            }

            return default(T);
        }
    }
}