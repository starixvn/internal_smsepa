// ----------------------------------------------------------------------
// <copyright file="BaseService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 2:27:29 PM">Created.</change>
// </history>

namespace SMS.EPA.Core.Services
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Repositories;

    /// <summary>
    /// The base service.
    /// </summary>
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

        /// <summary>
        /// Function to get list object from database.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="criteria">Criteria to query database.</param>
        /// <returns>List Object.</returns>
        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            return _dynamicRepository.Fetch<T>(criteria);
        }

        /// <summary>
        /// Function to get object from database.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="criteria">Criteria to query database.</param>
        /// <returns>The object.</returns>
        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            return _dynamicRepository.Get<T>(criteria);
        }
    }
}
