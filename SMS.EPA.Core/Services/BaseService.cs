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
            return dynamicRepository.Fetch<T>(criteria);
        }

        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            return dynamicRepository.Get<T>(criteria);
        }
    }
}