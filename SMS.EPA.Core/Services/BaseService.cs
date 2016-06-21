namespace SMS.EPA.Core.Services
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Repositories;

    public class BaseService
    {
        private IBaseRepository _repository;
        private IDynamicRepository _dynamicRepository;

        public BaseService(IDynamicRepository dynamicRepository = null, IBaseRepository repository = null)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository
        {
            get
            {
                return _repository;
            }
        }

        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            return _dynamicRepository.Fetch<T>(criteria);
        }

        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            return _dynamicRepository.Get<T>(criteria);
        }
    }
}