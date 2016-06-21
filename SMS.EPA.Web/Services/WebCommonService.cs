namespace SMS.EPA.Web.Services
{
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.RoleManagement;

    public class WebCommonService : IWebCommonService
    {
        public int GetPermission(string functionKey)
        {
            int policy = (int)Policy.None;

            if (HasPermissionOnFunction(functionKey, Policy.Host))
            {
                policy = (int)Policy.Host;
            }
            else if (HasPermissionOnFunction(functionKey, Policy.Read))
            {
                policy = (int)Policy.Read;
            }

            return policy;
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            if (SessionManager.CurrentUser != null)
            {
                FunctionModel currentFunction = SessionManager.CurrentUser.Functions
                            .Where(func => func.FunctionKey.Equals(functionKey)).FirstOrDefault();

                if (currentFunction != null)
                {
                    switch (policy)
                    {
                        case Policy.None:
                            return false;

                        case Policy.Read:
                            return currentFunction.IsRead || currentFunction.IsHost;

                        case Policy.Host:
                            return currentFunction.IsHost;
                    }
                }
            }

            return false;
        }
    }
}