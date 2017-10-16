namespace SMS.EPA.Core.Services
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Repositories;

    public class BaseService
    {
        private readonly IDynamicRepository _dynamicRepository;

        public BaseService(IDynamicRepository dynamicRepository = null, IBaseRepository repository = null)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            Repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository { get; set; }

        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return _dynamicRepository.Fetch<T>(criteria);
            }

            return new List<T>();
        }

        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return _dynamicRepository.Get<T>(criteria);
            }

            return default(T);
        }
    }
}