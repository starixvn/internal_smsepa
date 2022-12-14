namespace SMS.EPA.ClientHRM.Criteria
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.Constants;
    using EPA.Model.API;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateUserMappingCriteria : CriteriaBase
    {
        private const string UserMappingTpl = "<Row UserId='{0}' RefId='{1}' NewUserId='{2}' ModifiedDate='{3}' />";

        [SpParam(Ignored = true)]
        public IEnumerable<UserMapping> UserMappings { get; set; }

        public string UserMappingXml
        {
            get
            {
                var value = new StringBuilder(Constants.BeginRootTag);

                if (UserMappings != null && UserMappings.Any())
                {
                    foreach (var userMapping in UserMappings)
                    {
                        value.AppendFormat(
                            UserMappingTpl,
                            userMapping.UserId,
                            userMapping.RefId,
                            userMapping.NewUserId,
                            userMapping.ModifiedDate.HasValue ? userMapping.ModifiedDate.Value.ToString(Constants.ModifiedDateTimeFormat) : string.Empty
                        );
                    }
                }

                return value.Append(Constants.EndRootTag).ToString();
            }
        }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "Sync_UserMapping";

        public override bool IsValid() => UserMappings != null && UserMappings.Any();
    }
}
