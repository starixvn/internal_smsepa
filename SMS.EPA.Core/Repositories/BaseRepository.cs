namespace SMS.EPA.Core.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections;

    public class BaseRepository : IBaseRepository
    {
        public CommandResult Insert(ICriteria criteria)
        {
            return ExecuteNonQuery(criteria);
        }

        public CommandResult Update(ICriteria criteria)
        {
            return ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(ICriteria criteria)
        {
            return ExecuteNonQuery(criteria);
        }

        private CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();
                hash.Add("ErrCode", 0);
                hash.Add("ErrMsg", string.Empty);

                db.ExecuteNonQuery(hash);
                CommandResult commandResult = GetCommandResult(hash);

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
    }
}