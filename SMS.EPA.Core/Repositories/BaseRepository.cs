// ----------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 2:32:41 PM">Created.</change>
// </history>

namespace SMS.EPA.Core.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Criteria;
    using SMS.EPA.Core.Model;

    /// <summary>
    /// The base repository.
    /// </summary>
    public class BaseRepository : IBaseRepository
    {
        public CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(criteria, hasGetOutputParams);
        }

        public CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(criteria, hasGetOutputParams);
        }

        public CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(criteria, hasGetOutputParams);
        }

        /// <summary>
        /// Function to insert, update or delete.
        /// </summary>
        /// <param name="criteria">Criteria contains parameter throw stored procedure.</param>
        /// <param name="hasGetOutputParams">Has get output parameters or no.</param>
        /// <returns>Command Result.</returns>
        private CommandResult ExecuteNonQuery(ICriteria criteria, bool hasGetOutputParams = false)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                int result = db.ExecuteNonQuery(hash);
                CommandResult commandResult = GetCommandResult(hash);

                if (result == -1 && commandResult.IsSuccessful && hasGetOutputParams)
                {
                    SetValueForOutputProperty(criteria, hash);
                }

                return commandResult;
            }
        }

        private CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }

        /// <summary>
        /// Function to set new value from Hash table to Criteria.
        /// </summary>
        /// <param name="criteria">The Criteria.</param>
        /// <param name="hash">The Hash table.</param>
        private void SetValueForOutputProperty(ICriteria criteria, Hashtable hash)
        {
            IEnumerable<PropertyInfo> properties = criteria.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(OutputAttribute)));

            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    var value = hash[property.Name.ToLower()];
                    property.SetValue(criteria, value, null);
                }
            }
        }
    }
}
