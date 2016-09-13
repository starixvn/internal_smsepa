namespace SMS.EPA.Core.Services
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Repositories;

    public class BaseService
    {
        private IBaseRepository repository;
        private IDynamicRepository dynamicRepository;

        public BaseService(IDynamicRepository dynamicRepository = null, IBaseRepository repository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository
        {
            get
            {
                return repository;
            }

            set
            {
                repository = value;
            }
        }

        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Fetch<T>(criteria);
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
    }
}