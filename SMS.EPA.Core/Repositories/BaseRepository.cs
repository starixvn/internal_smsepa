namespace SMS.EPA.Core.Repositories
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class BaseRepository : IBaseRepository
    {
        public virtual CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.SetParameterValue("ErrCode", 0);
                db.SetParameterValue("ErrMsg", string.Empty);

                db.ExecuteNonQuery(criteria);
                Hashtable hash = db.GetOutputParams();

                return GetCommandResult(hash);
            }
        }

        public virtual async Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.SetParameterValue("ErrCode", 0);
                db.SetParameterValue("ErrMsg", string.Empty);

                await db.ExecuteNonQueryAsync(criteria);
                Hashtable hash = db.GetOutputParams();

                return GetCommandResult(hash);
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